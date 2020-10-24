using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

using NUnit.Framework;
using log4net;
using log4net.Config;
using iaik.pkcs.pkcs11.wrapper;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.util;
using tr.gov.tubitak.uekae.esya.api.smartcard.gui;
using tr.gov.tubitak.uekae.esya.api.smartcard;
using tr.gov.tubitak.uekae.esya.api.common.util.bag;
using tr.gov.tubitak.uekae.esya.api.common;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.smartcard.pkcs11;
using tr.gov.tubitak.uekae.esya.api.certificate.validation;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.transforms;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.keyinfo;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.keyinfo.x509;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades;
using tr.gov.tubitak.uekae.esya.asn.util;
//using xmlsig.samples.utils;
//using xmlsig.samples.validation;
using System.Security.Cryptography.X509Certificates;

using XmlUtil = tr.gov.tubitak.uekae.esya.api.xmlsignature.util.XmlUtil;
using LV = tr.gov.tubitak.uekae.esya.api.xmlsignature.Context;

using LicenseUtil = tr.gov.tubitak.uekae.esya.api.common.util.LicenseUtil;
using X509Certificate2 = System.Security.Cryptography.X509Certificates.X509Certificate2;
using TransformType = tr.gov.tubitak.uekae.esya.api.xmlsignature.TransformType;
using Config = tr.gov.tubitak.uekae.esya.api.xmlsignature.config.Config;

namespace ubltr
{
    public static class ublSigner
    {
        public static readonly ILog LOGGER = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static IBaseSmartCard bsc;
        public static BaseSigner mSigner;
        public static string cardPinNo = "";

        public static readonly bool WORK_ONLY_WITH_QUALIFIED_CERTS = false;
        public static int askOption(Control aParent, Icon aIcon, String[] aSecenekList, String aBaslik,
                                     String[] aOptions)
        {
            SlotList sl = new SlotList(null, aIcon, aSecenekList, aBaslik);
            DialogResult result = sl.ShowDialog();
            if (result != DialogResult.OK)
                return -1;
            return sl.getSelectedIndex();
        }

        public static void TestEnvelopedSignatureInitialize()
        {
            XmlConfigurator.Configure(new FileInfo(Conn.ROOT_DIR + "efatura\\config\\log4net.xml"));
            LicenseUtil.setLicenseXml(new FileStream(Conn.ROOT_DIR + "efatura\\lisans\\BES_Lisans.xml", FileMode.Open, FileAccess.Read));
        }

        public static bool createEnvelopedBes(string pinNo, string signXML, String outXML, bool bInTest)
        {
            bool res = false;
            cardPinNo = pinNo;
            TestEnvelopedSignatureInitialize();
            try
            {
                // here is our custom envelope xml
                //  XmlDocument envelopeDoc = newEnvelope("edefter.xml");


                XmlDocument envelopeDoc = Conn.newEnvelope(signXML);
                XmlElement exts = (XmlElement)envelopeDoc.GetElementsByTagName("ext:UBLExtensions").Item(0);
                XmlElement ext = (XmlElement)exts.GetElementsByTagName("ext:UBLExtension").Item(0);
                XmlElement extContent = (XmlElement)ext.GetElementsByTagName("ext:ExtensionContent").Item(0);
                UriBuilder ub = new UriBuilder(Conn.ROOT_DIR + "efatura\\config\\");
                // create context with working dir
                Context context = new Context(ub.Uri);

                //UriBuilder ub2 = new UriBuilder(Conn.ROOT_DIR + "efatura\\config\\xmlsignature-config.xml");
                context.Config = new Config(Conn.ROOT_DIR + "efatura\\config\\xmlsignature-config.xml");

                // define where signature belongs to
                context.Document = envelopeDoc;

                // create signature according to context,
                // with default type (XADES_BES)
                XMLSignature signature = new XMLSignature(context, false);

                String setID = "Signature_" + envelopeDoc.GetElementsByTagName("cbc:ID").Item(0).InnerText;
                signature.Id = setID;
                signature.SigningTime = DateTime.Now;

                // attach signature to envelope
                //envelopeDoc.DocumentElement.AppendChild(signature.Element);
                extContent.AppendChild(signature.Element);

                //add transforms for efatura
                Transforms transforms = new Transforms(context);
                transforms.addTransform(new Transform(context, TransformType.ENVELOPED.Url));


                // add document as reference,
                //signature.addDocument("#data1", "text/xml", false);
                signature.addDocument("", "text/xml", transforms, DigestMethod.SHA_256, false);

                ECertificate certificate = SmartCardManager.getInstance().getEInvoiceCertificate(cardPinNo);// getSignatureCertificate(true, false);
                if (certificate.isMaliMuhurCertificate())
                {
                    ValidationPolicy policy = new ValidationPolicy();
                    String policyPath = Conn.ROOT_DIR + "efatura\\config\\certval-policy-malimuhur.xml";
                    policy = PolicyReader.readValidationPolicy(policyPath);
                    ValidationSystem vs = CertificateValidation.createValidationSystem(policy);
                    context.setCertValidationSystem(vs);
                }
                else
                {
                    ValidationPolicy policy = new ValidationPolicy();
                    String policyPath = Conn.ROOT_DIR + "efatura\\config\\certval-policy.xml";
                    policy = PolicyReader.readValidationPolicy(policyPath);
                    ValidationSystem vs = CertificateValidation.createValidationSystem(policy);
                    context.setCertValidationSystem(vs);
                }

                if (CertValidation.validateCertificate(certificate) || bInTest)
                {
                    BaseSigner signer = SmartCardManager.getInstance().getSigner(cardPinNo, certificate);

                    X509Certificate2 msCert = certificate.asX509Certificate2();
                    signature.addKeyInfo(msCert.PublicKey.Key);
                    signature.addKeyInfo(certificate);

                    KeyInfo keyInfo = signature.createOrGetKeyInfo();
                    int elementCount = keyInfo.ElementCount;
                    for (int k = 0; k < elementCount; k++)
                    {
                        KeyInfoElement kiElement = keyInfo.get(k);
                        if (kiElement.GetType().IsAssignableFrom(typeof(X509Data)))
                        {
                            X509Data x509Data = (X509Data)kiElement;
                            X509SubjectName x509SubjectName = new X509SubjectName(context,
                                                                                  certificate.getSubject().stringValue());
                            x509Data.add(x509SubjectName);
                            break;
                        }
                    }

                    //signature.addKeyInfo(certificate);

                    signature.SignedInfo.CanonicalizationMethod = C14nMethod.EXCLUSIVE_WITH_COMMENTS;

                    signature.sign(signer);

                    // this time we dont use signature.write because we need to write
                    // whole document instead of signature
                    using (Stream s = new FileStream(outXML, FileMode.Create))
                    {
                        try
                        {
                            envelopeDoc.Save(s);
                            s.Flush();
                            s.Close();

                            res = true;
                        }
                        catch (Exception e)
                        {
                            res = false;
                            MessageBox.Show("Dosya kaydedilirken hata oluştu " + e.Message.ToString());
                            s.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                res = false;
                MessageBox.Show("Hata Oluştu \r\n" + e.Message.ToString());
            }
                
            return res;
        }

    }
}
