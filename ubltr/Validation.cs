using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using log4net.Config;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.certificate.validation;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.document;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.model.xades;
//using xmlsig.samples.utils;

namespace ubltr
{
    /**
     * Provides validating functions for xml signatures.
     * @author suleyman.uslu
     */
    //[TestFixture]
    public class Validation : SampleBase
    {
        public static void setUp()
        {
            XmlConfigurator.Configure(new FileInfo(Conn.ROOT_DIR + "efatura\\config\\log4net.xml"));
            loadLicense();
        }

        /**
         * Generic validate function. Validates known types of xml signature.
         * @param fileName name of the signature file to be validated
         */
        public static void validate(String fileName)
        {
            Context context = new Context(Conn.ROOT_DIR + "efatura\\config\\");

            // add external resolver to resolve policies
            context.addExternalResolver(getPolicyResolver());

            XMLSignature signature = XMLSignature.parse(
                                        new FileDocument(new FileInfo(fileName)),
                                        context);

            ECertificate cert = signature.SigningCertificate;
            ValidationSystem vs;

            if (cert.isMaliMuhurCertificate())
            {
                ValidationPolicy policy = new ValidationPolicy();
                String policyPath = Conn.ROOT_DIR + "efatura\\config\\certval-policy-malimuhur.xml";
                policy = PolicyReader.readValidationPolicy(policyPath);
                vs = CertificateValidation.createValidationSystem(policy);
                context.setCertValidationSystem(vs);
            }
            else
            {
                ValidationPolicy policy = new ValidationPolicy();
                String policyPath = Conn.ROOT_DIR + "efatura\\config\\certval-policy.xml";
                policy = PolicyReader.readValidationPolicy(policyPath);
                vs = CertificateValidation.createValidationSystem(policy);
                context.setCertValidationSystem(vs);
            }

            // no params, use the certificate in key info
            ValidationResult result = signature.verify();
            String sonuc = result.toXml();
            Console.WriteLine(result.toXml());
            // Assert.True(result.Type == ValidationResultType.VALID,"Cant verify " + fileName);

            UnsignedSignatureProperties usp = signature.QualifyingProperties.UnsignedSignatureProperties;
            if (usp != null)
            {
                IList<XMLSignature> counterSignatures = usp.AllCounterSignatures;
                foreach (XMLSignature counterSignature in counterSignatures)
                {
                    ValidationResult counterResult = signature.verify();

                    Console.WriteLine(counterResult.toXml());

                    //Assert.True(counterResult.Type == ValidationResultType.VALID,
                    //    "Cant verify counter signature" + fileName + " : "+counterSignature.Id);

                }
            }
        }

        /**
         * Validate function for parallel signatures
         * @param fileName name of the signature file to be validated
         */
        public static void validateParallel(String fileName)
        {
            Context context = new Context(Conn.ROOT_DIR + "efatura\\config\\");

            // add external resolver to resolve policies
            context.addExternalResolver(getPolicyResolver());

            List<XMLSignature> xmlSignatures = new List<XMLSignature>();

            // get signature as signed document in order to be able to validate parallel ones
            SignedDocument sd = new SignedDocument(new FileDocument(new FileInfo( fileName )), context);

            xmlSignatures.AddRange(sd.getRootSignatures());

            foreach (var xmlSignature in xmlSignatures)
            {
                // no params, use the certificate in key info
                ValidationResult result = xmlSignature.verify();
                Console.WriteLine(result.toXml());
                Assert.True(result.getType() == ValidationResultType.VALID, "Cant verify " + fileName);

                UnsignedSignatureProperties usp = xmlSignature.QualifyingProperties.UnsignedSignatureProperties;
                if (usp != null)
                {
                    IList<XMLSignature> counterSignatures = usp.AllCounterSignatures;
                    foreach (XMLSignature counterSignature in counterSignatures)
                    {
                        ValidationResult counterResult = xmlSignature.verify();

                        Console.WriteLine(counterResult.toXml());

                        Assert.True(counterResult.getType() == ValidationResultType.VALID,
                            "Cant verify counter signature" + fileName + " : " + counterSignature.Id);

                    }
                }
            }

        }
    }
}
