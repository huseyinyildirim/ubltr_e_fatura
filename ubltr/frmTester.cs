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
    public partial class frmTester : Form
    {
        public frmTester()
        {
            InitializeComponent();
        }

        private void frmTester_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ublSigner.createEnvelopedBes(
                txtPinNumber.Text.ToString(),
                txtSignXML.Text.ToString(),
                txtOutputXML.Text.ToString(),
                cbInTest.Checked);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnLoadSignXML_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Title = "İmzalanacak XML i Seçiniz";
            dlgOpenFile.Filter = "*.XML|*.xml";
            DialogResult dr = dlgOpenFile.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtSignXML.Text = dlgOpenFile.FileName.ToString();
            }
        }

        private void btnSetSignXML_Click(object sender, EventArgs e)
        {
            dlgSaveFile.Title = "İmzalanan Dosyanın Kaydedileceği XML i Seçiniz";
            dlgSaveFile.Filter = "*.XML|*.xml";
            DialogResult dr = dlgSaveFile.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtOutputXML.Text = dlgSaveFile.FileName.ToString();
            }
        }

        private void btnSetConfigPath_Click(object sender, EventArgs e)
        {
            DialogResult dr = dlgChooseFolder.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtConfigXmlPath.Text = dlgChooseFolder.SelectedPath.ToString();
            }
        }

        private void btnSetLicencePath_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Title = "Lisans XML i Seçiniz";
            dlgOpenFile.Filter = "*.XML|*.xml";
            DialogResult dr = dlgOpenFile.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtLicensePath.Text = dlgOpenFile.FileName.ToString();
            }
        }
    }
}
