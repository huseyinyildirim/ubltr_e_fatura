using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using MySql.Data.MySqlClient;
using System.IO.Packaging;
using System.Net;
using System.Diagnostics;
using System.ServiceModel;

/*
 * todo list
 * +zip error
 * +schema validation
 * -faturano değiştirince sadece fatura dosyası güncelleniyor.
 *  muhasebe hareket , ayrilankonaklama bilgileri ve kimliklerin de güncellenmesi gerekiyor
 * -correction olunca - rakam atıyor
 * -detay satırları türkçe karakter problemi
 * 
*/

namespace ubltr
{
    public partial class frmInvoice : Form
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        MySqlDataAdapter da;
        DataTable dt;
        MySqlCommandBuilder cb;

        const string sqlCODetay = "select " +
                                " aciklama,kdvsiztutar,kdvtutari,kdvlitutar,iskorani/100 as iskontoorani, kdvliisktutar," +
                                " kdvsizisktutar,kdvorani*100 as kdvorani,1 as miktar,  kdvsiztutar as fiyat," +
                                " '' as satiraciklamasi from checkoutfaturadetay  where faturaid='{0}' " +
                                " and kdvsiztutar>0 order by sira";

        const string sqlAFDetay = "select aciklama,kdvsiztutar,kdvtutari,kdvlitutar,iskorani/100 as iskontoorani," +
                                " kdvliisktutar,kdvsizisktutar,kdvorani*100 as kdvorani,1 as miktar," +
                                " kdvsiztutar as fiyat,'' as satiraciklamasi from acentafaturadetay " +
                                " where faturaid='{0}' and kdvsiztutar>0 order by sira";
        const string sqlAFDetay2 = "select " +
                                    " id, gelistarihi, ayrilistarihi, acentafolyono, voucherno, adi, soyadi, sum(yetiskin), " +
                                    " toplamtloda, toplamdovizoda, dvz, kurucreti, " +
                                    " group_concat(concat( " +
                                    "  date_format(bas,'%d'), '-', date_format(bitis,'%d/%m/%Y'),' (',datediff(bitis,bas)+1,') ', " +
                                    "  if(ifnull(toplamdovizoda,0)>0 and dvz<>'TL' , " +
                                    "     concat(iround(toplamdovizoda,2),' ',dvz,'(',iround(kurucreti,4),') ',iround(toplamtloda,2),' TL'), " +
                                    "     concat( iround(toplamtloda,2), ' TL' ) ), " +
                                    "  if(extra>0, concat(' + ',iround(extra,2),' TL'), '') " +
                                    " )) as detay " +
                                    "from ( " +
                                    "select " +
                                    " ak.id, ak.gelistarihi, ak.ayrilistarihi, ak.acentafolyono, ak.voucherno, ak.adi, ak.soyadi, ak.kisisayisi, " +
                                    " aaok.doviztutari, aaok.ytltutari, sum(aaok.ytltutari) as toplamtloda, sum(aaok.doviztutari) as toplamdovizoda, " +
                                    " min(aaok.tarih) as bas, max(aaok.tarih) as bitis, " +
                                    " aaok.yetiskinsayisi as yetiskin, " +
                                    " aaok.dovizcinsi as dvz, " +
                                    " aaok.kurucreti, " +
                                    " ifnull((select sum(ifnull(ytltutari,0)) from ayrilanacentaadisyonucretleri where checkinid=ak.id),0) as extra " +
                                    "from ayrilankonaklamabilgileri as ak " +
                                    "left join ayrilanacentaodakalisucretleri as aaok on aaok.checkinid=ak.id " +
                                    "where ak.acentafolyono={0} " +
                                    "group by ak.id, aaok.dovizcinsi, aaok.ytltutari " +
                                    ") as f " +
                                    "group by id ";

        const string sqlCFDetay = "";

        /*todo
         * -:birim fiyat ve miktarlar ile tutar kdvli kdvsiz gelsin
         * 
         */
        const string sqlSFDetay = "select *,kdvsiztutar as fiyat,'' satiraciklamasi from (select " +
                                " sira,aciklama, " +
                                " if(kdvsekli=1, Tutar - ifnull(isktutari,0) , (tutar / (1+ (kdvorani/100))) - ifnull(isktutari,0) ) as kdvsiztutar, " +
                                " if(kdvsekli=1, ((tutar * (kdvorani/100)) - ifnull(isktutari*(kdvorani/100),0)), tutar - ((tutar/(1+(kdvorani/100)))-(ifnull(isktutari,0))) ) kdvtutari, " +
                                " if(kdvsekli=1, ( tutar * (1+(kdvorani/100)) ) - ifnull(isktutari,0), tutar  - ifnull(isktutari,0) ) as kdvlitutar, " +
                                " iskorani/100 as iskontoorani, " +
                                " if(kdvsekli=1, ifnull(isktutari,0) * (1+(kdvorani/100)), ifnull(isktutari,0))kdvliisktutar, " +
                                " if(kdvsekli=1, ifnull(isktutari,0), ifnull(isktutari,0)-(ifnull(isktutari,0)* (kdvorani/100)) ) as kdvsizisktutar, " +
                                " kdvorani as kdvorani, " +
                                " 1 as miktar " +
                                "from sifatura_detay " +
                                "where fid='{0}' and tutar>0" +
                                ") as f " +
                                "order by sira";

        const string sqlAGDetay = "";

        string myTaxNumber, myAddressRoom, myAddressStreet, myAddressBuildingName, myAddressBuildingNumber, myAddressCitySubDivision,
               myAddressCityName, myAddressPostalZone, myAddressRegion, myAddressCountry, myCompanyName, myTaxOffice, myTelephone,
               myFax, myEmail, myWeb, myInvoiceNotes, myTradeNumber, myMersisNumber, pinNo = "";

        List<string> imzalananFaturalar = new List<string>();

        public List<KDV> kdvler = new List<KDV>();

        public void CreateZipFile(string filename)
        {
            //Create the header of the Zip File 
            System.Text.ASCIIEncoding Encoder = new System.Text.ASCIIEncoding();
            string sHeader = "PK" + (char)5 + (char)6;
            sHeader = sHeader.PadRight(22, (char)0);
            //Convert to byte array
            byte[] baHeader = System.Text.Encoding.ASCII.GetBytes(sHeader);

            //Save File - Make sure your file ends with .zip!
            FileStream fs = File.Create(filename);
            fs.Write(baHeader, 0, baHeader.Length);
            fs.Flush();
            fs.Close();
            fs = null;
        }

        public bool myCreateZipFile(String fileName, List<String> files)
        {
            bool res = false;
            Shell32.Shell Shell = new Shell32.Shell();

            //Create our Zip File
            CreateZipFile(fileName);
            foreach (string fname in files)
            {
                //Copy the file or folder to it
                Shell.NameSpace(fileName).CopyHere(fname, 0);
                System.Threading.Thread.Sleep(200);
            }
            //If you can write the code to wait for the code to finish, please let me know
            System.Threading.Thread.Sleep(2000);
            res = true;

            return res;

        }

        public frmInvoice()
        {
            InitializeComponent();
        }

        public int KdvEkle(double kdvorani, double kdvtutari, double kdvmatrahi, double kdvlitutar)
        {
            int res = -1;
            for (int i = 0; i < kdvler.Count; i++)
            {
                KDV k = kdvler[i];
                if (k.kdvOrani == kdvorani)
                {
                    k.kdvliTutar += kdvlitutar;
                    k.kdvMatrahi += kdvmatrahi;
                    k.kdvTutari += kdvtutari;
                    res = i;
                    break;
                }
            }
            if (res == -1)
            {
                KDV k = new KDV();
                k.kdvOrani = kdvorani;
                k.kdvliTutar = kdvlitutar;
                k.kdvMatrahi = kdvmatrahi;
                k.kdvTutari = kdvtutari;
                kdvler.Add(k);
                res = kdvler.Count - 1;
            }
            return res;
        }

        public bool CreateUBL(string myInvoiceID, string myInvoiceType, ref string refInvoiceNumber)
        {
            bool res = false;
            Conn.OpenConn();
            Properties.Settings mySettings = new Properties.Settings();
            string sql = "",
                   sqlDetay = "",
                   myInvoiceNumber,
                   invoiceTypeCode = "",
                   invoiceTypeVal = "",
                   invoiceFile = "",
                   saveXmlFolder = Conn.SaveXmlPath,
                   saveXmlfileName = "",
                   saveXmlfileNameTemp = "",
                   kdvIstisnasiNotu = "";

            if (!Directory.Exists(saveXmlFolder)) Directory.CreateDirectory(saveXmlFolder);

            //bool fatNotVar = false;
            try
            {
                string acentafolyono = "", sqlafdetay = "";
                sql = String.Format("select * from efaturabilgi where faturatipi='{0}' and faturaid={1}", myInvoiceType, myInvoiceID);
                MySqlDataAdapter fb = Conn.OpenAdapter(sql);
                DataTable dtfb = new DataTable();
                fb.Fill(dtfb);
                if (dtfb.Rows.Count > 0)
                {
                    if (myInvoiceType == "CO")
                    {
                        sql = String.Format("select faturano,tarih,kdv,iskonto,toplam as geneltoplam,0 as faturatipi from checkoutfatura where id={0}", myInvoiceID);
                        sqlDetay = String.Format(sqlCODetay, myInvoiceID);
                        invoiceFile = "checkoutfatura";
                        string ckid = Conn.ReadSingleField("select ayrilanid from checkoutfatura where id='" + myInvoiceID + "'", "0");
                        //AD Soyad /giriş çıkış/voucher
                        //sqlKonaklamaDetay = string.Format("select concat('Room:',odano,', Name:',adi,' ',soyadi,', Vou./Fol.:',ifnull(voucherno,''),'/',odafolyono,"+
                        //                    " ', Arr./Dep.:',gelistarihi,'/',ayrilistarihi) as f from ayrilankonaklamabilgileri where id='{0}' ",ckid);
                        //fatNotVar = true;
                    }
                    else if (myInvoiceType == "AF")
                    {
                        sql = String.Format("select faturano,tarih,kdv,iskonto,geneltoplam,'0' as faturatipi from acentafatura where id={0}", myInvoiceID);
                        sqlDetay = String.Format(sqlAFDetay, myInvoiceID);
                        invoiceFile = "acentafatura";
                        acentafolyono = Conn.ReadSingleField("select acentafolyono from acentafatura where id='" + myInvoiceID + "'", "0");
                        //tüm odalar için AD Soyad /giriş çıkış/voucher
                        sqlafdetay = string.Format(sqlAFDetay2, acentafolyono);
                        //fatNotVar = true;
                    }
                    else if (myInvoiceType == "SF")
                    {
                        sql = String.Format("select faturano,tarih,kdv,iskonto,geneltoplam,cast(faturatipi as char) as faturatipi from sifatura where id={0}", myInvoiceID);
                        sqlDetay = String.Format(sqlSFDetay, myInvoiceID);
                        invoiceFile = "sifatura";
                    }
                    else if (myInvoiceType == "CF")
                    {
                        sql = String.Format("select faturano,tarih,kdv,iskonto,geneltoplam,'0' as faturatipi from carifatura where id={0}", myInvoiceID);
                        sqlDetay = String.Format(sqlCFDetay, myInvoiceID);
                        invoiceFile = "carifatura";
                    }
                    else if (myInvoiceType == "AG")
                    {
                        sql = String.Format("select faturano,tarih,kdv,iskonto,geneltoplam,'0' as faturatipi from adisyonfaturasi where id={0}", myInvoiceID);
                        sqlDetay = String.Format(sqlAGDetay, myInvoiceID);
                        invoiceFile = "adisyonfaturasi";
                    }

                    MySqlDataAdapter inv = Conn.OpenAdapter(sql);
                    DataTable dtinv = new DataTable();
                    inv.Fill(dtinv);

                    MySqlDataAdapter invdet = Conn.OpenAdapter(sqlDetay);
                    DataTable dtinvdet = new DataTable();
                    invdet.Fill(dtinvdet);

                    MySqlDataAdapter kondet;
                    DataTable dkondetdet = null;


                    if (sqlafdetay != "")
                    {
                        kondet = Conn.OpenAdapter(sqlafdetay);
                        dkondetdet = new DataTable();
                        kondet.Fill(dkondetdet);
                    }

                    if (dtinv.Rows.Count > 0 && dtinvdet.Rows.Count > 0)
                    {
                        kdvler.Clear();
                        invoiceTypeVal = dtinv.Rows[0]["faturatipi"].ToString();
                        invoiceTypeCode = "SATIS";
                        if (invoiceTypeVal == "2" || invoiceTypeVal == "3") invoiceTypeCode = "IADE";

                        myInvoiceNumber = dtinv.Rows[0]["FaturaNo"].ToString();
                        refInvoiceNumber = myInvoiceNumber;
                        saveXmlfileName = saveXmlFolder + myInvoiceNumber + ".xml";
                        saveXmlfileNameTemp = saveXmlFolder + "_" + myInvoiceNumber + ".xml";
                        //MessageBox.Show(s.MysqlDB.ToString());
                        sql = String.Format("update {0} set efatura=-2 where id={1}", invoiceFile, myInvoiceID);
                        Conn.ExecuteSql(sql);

                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("xades", "http://uri.etsi.org/01903/v1.3.2#");

                        ns.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
                        ns.Add("ubltr", "urn:oasis:names:specification:ubl:schema:xsd:TurkishCustomizationExtensionComponents");
                        ns.Add("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                        ns.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                        ns.Add("ccts", "urn:un:unece:uncefact:documentation:2");
                        ns.Add("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                        ns.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                        ns.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");

                        XmlSerializer mySerializer = new XmlSerializer(typeof(ublXSD.InvoiceType));
                        ublXSD.InvoiceType myInvoice = new ublXSD.InvoiceType();

                        myInvoice.UBLExtensions = new ublXSD.UBLExtensionsType[1];
                        myInvoice.UBLExtensions[0] = new ublXSD.UBLExtensionsType();
                        myInvoice.UBLExtensions[0].UBLExtension = new ublXSD.UBLExtensionType();
                        myInvoice.UBLExtensions[0].UBLExtension.ExtensionContent = new ublXSD.ExtensionContentType();

                        myInvoice.CopyIndicator = new ublXSD.CopyIndicatorType();
                        myInvoice.CopyIndicator.Value = false;

                        myInvoice.UBLVersionID = new ublXSD.UBLVersionIDType();
                        myInvoice.UBLVersionID.Value = "2.0";

                        myInvoice.CustomizationID = new ublXSD.CustomizationIDType();
                        myInvoice.CustomizationID.Value = "TR1.0";

                        myInvoice.ProfileID = new ublXSD.ProfileIDType();
                        myInvoice.ProfileID.Value = Conn.convert2UTF(dtfb.Rows[0]["efaturasekli"].ToString());

                        myInvoice.ID = new ublXSD.IDType();
                        myInvoice.ID.Value = myInvoiceNumber;

                        Guid g = Guid.NewGuid();
                        sql = String.Format("update efaturabilgi set guid='{0}' where faturatipi='{1}' and faturaid={2}", g.ToString(), myInvoiceType, myInvoiceID);
                        Conn.ExecuteSql(sql);

                        myInvoice.UUID = new ublXSD.UUIDType();
                        myInvoice.UUID.Value = g.ToString();

                        myInvoice.IssueDate = new ublXSD.IssueDateType();
                        myInvoice.IssueDate.Value = DateTime.Parse(dtinv.Rows[0]["tarih"].ToString());

                        myInvoice.InvoiceTypeCode = new ublXSD.InvoiceTypeCodeType();
                        myInvoice.InvoiceTypeCode.Value = invoiceTypeCode;

                        myInvoice.DocumentCurrencyCode = new ublXSD.DocumentCurrencyCodeType();
                        myInvoice.DocumentCurrencyCode.listID = "ISO 4217 Alpha";
                        myInvoice.DocumentCurrencyCode.listAgencyName = "United Nations Economic Commission for Europe";
                        myInvoice.DocumentCurrencyCode.listName = "Currency";
                        myInvoice.DocumentCurrencyCode.listVersionID = "2001";
                        myInvoice.DocumentCurrencyCode.Value = ublXSD.CurrencyCodeContentType.TRL;

                        string xslt = "";
                        using (StreamReader streamReader = new StreamReader(Conn.ROOT_DIR + "efatura\\efatura.xslt", Encoding.UTF8))
                        {
                            xslt = streamReader.ReadToEnd();
                        }
                        myInvoice.AdditionalDocumentReference = new ublXSD.DocumentReferenceType[1];
                        myInvoice.AdditionalDocumentReference[0] = new ublXSD.DocumentReferenceType();
                        myInvoice.AdditionalDocumentReference[0].ID = new ublXSD.IDType();
                        myInvoice.AdditionalDocumentReference[0].ID.Value = Guid.NewGuid().ToString();
                        myInvoice.AdditionalDocumentReference[0].IssueDate = new ublXSD.IssueDateType();
                        myInvoice.AdditionalDocumentReference[0].IssueDate.Value = DateTime.Parse(dtinv.Rows[0]["tarih"].ToString());
                        myInvoice.AdditionalDocumentReference[0].Attachment = new ublXSD.AttachmentType();
                        myInvoice.AdditionalDocumentReference[0].Attachment.EmbeddedDocumentBinaryObject = new ublXSD.EmbeddedDocumentBinaryObjectType();
                        myInvoice.AdditionalDocumentReference[0].Attachment.EmbeddedDocumentBinaryObject.filename = myInvoiceNumber + ".xslt";
                        myInvoice.AdditionalDocumentReference[0].Attachment.EmbeddedDocumentBinaryObject.encodingCode = "Base64";
                        myInvoice.AdditionalDocumentReference[0].Attachment.EmbeddedDocumentBinaryObject.characterSetCode = "UTF-8";
                        myInvoice.AdditionalDocumentReference[0].Attachment.EmbeddedDocumentBinaryObject.mimeCode = ublXSD.BinaryObjectMimeCodeContentType.applicationxml;
                        myInvoice.AdditionalDocumentReference[0].Attachment.EmbeddedDocumentBinaryObject.Value = Encoding.UTF8.GetBytes(xslt);

                        //AdditionalDocumentReference
                        myInvoice.Signature = new ublXSD.SignatureType1[1];
                        myInvoice.Signature[0] = new ublXSD.SignatureType1();
                        myInvoice.Signature[0].ID = new ublXSD.IDType();
                        myInvoice.Signature[0].ID.schemeID = "VKN_TCKN";
                        myInvoice.Signature[0].ID.Value = myTaxNumber.ToString();

                        myInvoice.Signature[0].SignatoryParty = new ublXSD.PartyType();
                        myInvoice.Signature[0].SignatoryParty.PartyIdentification = new ublXSD.PartyIdentificationType[1];
                        myInvoice.Signature[0].SignatoryParty.PartyIdentification[0] = new ublXSD.PartyIdentificationType();
                        myInvoice.Signature[0].SignatoryParty.PartyIdentification[0].ID = new ublXSD.IDType();
                        myInvoice.Signature[0].SignatoryParty.PartyIdentification[0].ID.schemeID = "VKN";
                        myInvoice.Signature[0].SignatoryParty.PartyIdentification[0].ID.Value = myTaxNumber;
                        myInvoice.Signature[0].SignatoryParty.PostalAddress = new ublXSD.AddressType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.BuildingName = new ublXSD.BuildingNameType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.BuildingName.Value = Conn.XmlEscape(myAddressBuildingName);
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.BuildingNumber = new ublXSD.BuildingNumberType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.BuildingNumber.Value = Conn.XmlEscape(myAddressBuildingNumber);
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.CityName = new ublXSD.CityNameType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.CityName.Value = Conn.XmlEscape(myAddressCityName);
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.CitySubdivisionName = new ublXSD.CitySubdivisionNameType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.CitySubdivisionName.Value = Conn.XmlEscape(myAddressCitySubDivision);
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.PostalZone = new ublXSD.PostalZoneType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.PostalZone.Value = Conn.XmlEscape(myAddressPostalZone);
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.Region = new ublXSD.RegionType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.Region.Value = Conn.XmlEscape(myAddressRegion);
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.Room = new ublXSD.RoomType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.Room.Value = Conn.XmlEscape(myAddressRoom);
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.StreetName = new ublXSD.StreetNameType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.StreetName.Value = Conn.XmlEscape(myAddressStreet);
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.Country = new ublXSD.CountryType();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.Country.Name = new ublXSD.NameType1();
                        myInvoice.Signature[0].SignatoryParty.PostalAddress.Country.Name.Value = Conn.XmlEscape(myAddressCountry);
                        myInvoice.Signature[0].DigitalSignatureAttachment = new ublXSD.AttachmentType();
                        myInvoice.Signature[0].DigitalSignatureAttachment.ExternalReference = new ublXSD.ExternalReferenceType();
                        myInvoice.Signature[0].DigitalSignatureAttachment.ExternalReference.URI = new ublXSD.URIType();
                        myInvoice.Signature[0].DigitalSignatureAttachment.ExternalReference.URI.Value = "#Signature_" + myInvoiceNumber;


                        //todo: cac:Signature
                        //todo: cac:AccountingSupplierParty
                        myInvoice.AccountingSupplierParty = new ublXSD.SupplierPartyType();
                        myInvoice.AccountingSupplierParty.Party = new ublXSD.PartyType();
                        myInvoice.AccountingSupplierParty.Party.WebsiteURI = new ublXSD.WebsiteURIType();
                        myInvoice.AccountingSupplierParty.Party.WebsiteURI.Value = myWeb;
                        int iIDCount = 1;
                        if (myTradeNumber != "") iIDCount++;
                        if (myMersisNumber != "") iIDCount++;
                        myInvoice.AccountingSupplierParty.Party.PartyIdentification = new ublXSD.PartyIdentificationType[iIDCount];
                        int iidNo = 0;
                        myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo] = new ublXSD.PartyIdentificationType();
                        myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID = new ublXSD.IDType();
                        if (myTaxNumber.Length == 11)
                        {
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID.schemeID = "TCKN";
                        }
                        else
                        {
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID.schemeID = "VKN";
                        }
                        myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID.Value = myTaxNumber;

                        if (myTradeNumber != "")
                        {
                            iidNo++;
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo] = new ublXSD.PartyIdentificationType();
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID = new ublXSD.IDType();
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID.schemeID = "TICARETSICILNO";
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID.Value = myTradeNumber;
                        }

                        if (myMersisNumber != "")
                        {
                            iidNo++;
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo] = new ublXSD.PartyIdentificationType();
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID = new ublXSD.IDType();
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID.schemeID = "MERSISNO";
                            myInvoice.AccountingSupplierParty.Party.PartyIdentification[iidNo].ID.Value = myMersisNumber;
                        }


                        myInvoice.AccountingSupplierParty.Party.PartyName = new ublXSD.PartyNameType();
                        myInvoice.AccountingSupplierParty.Party.PartyName.Name = new ublXSD.NameType1();
                        myInvoice.AccountingSupplierParty.Party.PartyName.Name.Value = Conn.XmlEscape(myCompanyName);

                        //

                        myInvoice.AccountingSupplierParty.Party.PostalAddress = new ublXSD.AddressType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.Country = new ublXSD.CountryType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.Country.Name = new ublXSD.NameType1();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.Country.Name.Value = Conn.XmlEscape(myAddressCountry);

                        myInvoice.AccountingSupplierParty.Party.PostalAddress.BuildingName = new ublXSD.BuildingNameType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.BuildingName.Value = Conn.XmlEscape(myAddressBuildingName);
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.BuildingNumber = new ublXSD.BuildingNumberType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.BuildingNumber.Value = Conn.XmlEscape(myAddressBuildingNumber);
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.CityName = new ublXSD.CityNameType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.CityName.Value = Conn.XmlEscape(myAddressCityName);
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.CitySubdivisionName = new ublXSD.CitySubdivisionNameType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.CitySubdivisionName.Value = Conn.XmlEscape(myAddressCitySubDivision);
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.PostalZone = new ublXSD.PostalZoneType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.PostalZone.Value = Conn.XmlEscape(myAddressPostalZone);
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.Region = new ublXSD.RegionType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.Region.Value = Conn.XmlEscape(myAddressRegion);
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.Room = new ublXSD.RoomType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.Room.Value = Conn.XmlEscape(myAddressRoom);
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.StreetName = new ublXSD.StreetNameType();
                        myInvoice.AccountingSupplierParty.Party.PostalAddress.StreetName.Value = Conn.XmlEscape(myAddressStreet);

                        myInvoice.AccountingSupplierParty.Party.PartyTaxScheme = new ublXSD.PartyTaxSchemeType();
                        myInvoice.AccountingSupplierParty.Party.PartyTaxScheme.TaxScheme = new ublXSD.TaxSchemeType();
                        myInvoice.AccountingSupplierParty.Party.PartyTaxScheme.TaxScheme.Name = new ublXSD.NameType1();
                        myInvoice.AccountingSupplierParty.Party.PartyTaxScheme.TaxScheme.Name.Value = Conn.XmlEscape(myTaxOffice);

                        myInvoice.AccountingSupplierParty.Party.Contact = new ublXSD.ContactType();
                        myInvoice.AccountingSupplierParty.Party.Contact.ElectronicMail = new ublXSD.ElectronicMailType();
                        myInvoice.AccountingSupplierParty.Party.Contact.ElectronicMail.Value = Conn.XmlEscape(myEmail);
                        myInvoice.AccountingSupplierParty.Party.Contact.Telefax = new ublXSD.TelefaxType();
                        myInvoice.AccountingSupplierParty.Party.Contact.Telefax.Value = Conn.XmlEscape(myFax);
                        myInvoice.AccountingSupplierParty.Party.Contact.Telephone = new ublXSD.TelephoneType();
                        myInvoice.AccountingSupplierParty.Party.Contact.Telephone.Value = Conn.XmlEscape(myTelephone);

                        //todo: cac:AccountingCustomerParty

                        myInvoice.AccountingCustomerParty = new ublXSD.CustomerPartyType();
                        myInvoice.AccountingCustomerParty.Party = new ublXSD.PartyType();
                        myInvoice.AccountingCustomerParty.Party.WebsiteURI = new ublXSD.WebsiteURIType();
                        myInvoice.AccountingCustomerParty.Party.WebsiteURI.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaweb"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PartyIdentification = new ublXSD.PartyIdentificationType[1];
                        myInvoice.AccountingCustomerParty.Party.PartyIdentification[0] = new ublXSD.PartyIdentificationType();
                        myInvoice.AccountingCustomerParty.Party.PartyIdentification[0].ID = new ublXSD.IDType();
                        myInvoice.AccountingCustomerParty.Party.PartyIdentification[0].ID.Value = dtfb.Rows[0]["efaturaTcknVkn"].ToString();
                        if (dtfb.Rows[0]["efaturaTcknVkn"].ToString().Length == 11)
                        {
                            myInvoice.AccountingCustomerParty.Party.PartyIdentification[0].ID.schemeID = "TCKN";
                        }
                        else
                        {
                            myInvoice.AccountingCustomerParty.Party.PartyIdentification[0].ID.schemeID = "VKN";
                        }
                        myInvoice.AccountingCustomerParty.Party.PartyName = new ublXSD.PartyNameType();
                        myInvoice.AccountingCustomerParty.Party.PartyName.Name = new ublXSD.NameType1();
                        myInvoice.AccountingCustomerParty.Party.PartyName.Name.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaUnvan"].ToString()));

                        myInvoice.AccountingCustomerParty.Party.PostalAddress = new ublXSD.AddressType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.Country = new ublXSD.CountryType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.Country.Name = new ublXSD.NameType1();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.Country.Name.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaCountry"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingName = new ublXSD.BuildingNameType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingName.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaBuildingName"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingNumber = new ublXSD.BuildingNumberType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingNumber.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaBuildingNumber"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.CityName = new ublXSD.CityNameType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.CityName.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaCityName"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName = new ublXSD.CitySubdivisionNameType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaCitySubDivisionName"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.PostalZone = new ublXSD.PostalZoneType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.PostalZone.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaPostalZone"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.Region = new ublXSD.RegionType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.Region.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaRegion"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.Room = new ublXSD.RoomType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.Room.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaRoom"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.StreetName = new ublXSD.StreetNameType();
                        myInvoice.AccountingCustomerParty.Party.PostalAddress.StreetName.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaStreetName"].ToString()));

                        myInvoice.AccountingCustomerParty.Party.PartyTaxScheme = new ublXSD.PartyTaxSchemeType();
                        myInvoice.AccountingCustomerParty.Party.PartyTaxScheme.TaxScheme = new ublXSD.TaxSchemeType();
                        myInvoice.AccountingCustomerParty.Party.PartyTaxScheme.TaxScheme.Name = new ublXSD.NameType1();
                        myInvoice.AccountingCustomerParty.Party.PartyTaxScheme.TaxScheme.Name.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturavergidairesi"].ToString()));

                        myInvoice.AccountingCustomerParty.Party.Contact = new ublXSD.ContactType();
                        myInvoice.AccountingCustomerParty.Party.Contact.ElectronicMail = new ublXSD.ElectronicMailType();
                        myInvoice.AccountingCustomerParty.Party.Contact.ElectronicMail.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturaeposta"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.Contact.Telefax = new ublXSD.TelefaxType();
                        myInvoice.AccountingCustomerParty.Party.Contact.Telefax.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturafax"].ToString()));
                        myInvoice.AccountingCustomerParty.Party.Contact.Telephone = new ublXSD.TelephoneType();
                        myInvoice.AccountingCustomerParty.Party.Contact.Telephone.Value = Conn.XmlEscape(Conn.convert2UTF(dtfb.Rows[0]["efaturatel"].ToString()));
                        bool bKdvIstisnasi = false;
                        int itoplamSatir = 0;
                        //todo-:kdv istisnası nedenini ekrandan sor!
                        double toplamkdvsiziskonto = 0, toplamkdvsiz = 0, toplamkdv = 0; ;
                        myInvoice.InvoiceLine = new ublXSD.InvoiceLineType[dtinvdet.Rows.Count];
                        for (int iDetay = 0; iDetay < dtinvdet.Rows.Count; iDetay++)
                        {
                            KdvEkle(double.Parse(dtinvdet.Rows[iDetay]["kdvorani"].ToString()),
                                    double.Parse(dtinvdet.Rows[iDetay]["kdvtutari"].ToString()),
                                    double.Parse(dtinvdet.Rows[iDetay]["kdvsiztutar"].ToString()),
                                    double.Parse(dtinvdet.Rows[iDetay]["kdvlitutar"].ToString())
                                    );
                            toplamkdvsiziskonto += double.Parse(dtinvdet.Rows[iDetay]["kdvsizisktutar"].ToString());
                            toplamkdvsiz += double.Parse(dtinvdet.Rows[iDetay]["kdvsiztutar"].ToString());
                            toplamkdv += double.Parse(dtinvdet.Rows[iDetay]["kdvtutari"].ToString());

                            myInvoice.InvoiceLine[iDetay] = new ublXSD.InvoiceLineType();
                            myInvoice.InvoiceLine[iDetay].ID = new ublXSD.IDType();
                            myInvoice.InvoiceLine[iDetay].ID.Value = (iDetay + 1).ToString();
                            myInvoice.InvoiceLine[iDetay].InvoicedQuantity = new ublXSD.InvoicedQuantityType();
                            myInvoice.InvoiceLine[iDetay].InvoicedQuantity.unitCode = ublXSD.UnitCodeContentType.NIU;
                            myInvoice.InvoiceLine[iDetay].InvoicedQuantity.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["miktar"].ToString()).ToString("0.##"));
                            myInvoice.InvoiceLine[iDetay].LineExtensionAmount = new ublXSD.LineExtensionAmountType();
                            myInvoice.InvoiceLine[iDetay].LineExtensionAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                            myInvoice.InvoiceLine[iDetay].LineExtensionAmount.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["kdvsiztutar"].ToString()).ToString("0.##"));//kdvsiztutar
                            myInvoice.InvoiceLine[iDetay].Item = new ublXSD.ItemType();
                            myInvoice.InvoiceLine[iDetay].Item.Name = new ublXSD.NameType1();
                            myInvoice.InvoiceLine[iDetay].Item.Name.Value = Conn.convert2UTF(dtinvdet.Rows[iDetay]["aciklama"].ToString(), true);//ürün adı
                            myInvoice.InvoiceLine[iDetay].Item.SellersItemIdentification = new ublXSD.ItemIdentificationType();
                            myInvoice.InvoiceLine[iDetay].Item.SellersItemIdentification.ID = new ublXSD.IDType();
                            myInvoice.InvoiceLine[iDetay].Item.SellersItemIdentification.ID.Value = "";
                            myInvoice.InvoiceLine[iDetay].Price = new ublXSD.PriceType();
                            myInvoice.InvoiceLine[iDetay].Price.PriceAmount = new ublXSD.PriceAmountType();
                            myInvoice.InvoiceLine[iDetay].Price.PriceAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                            myInvoice.InvoiceLine[iDetay].Price.PriceAmount.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["kdvsiztutar"].ToString()).ToString("0.##")); ;//kdvsiztutar
                            //tvkifat ayrımı yapılacak
                            myInvoice.InvoiceLine[iDetay].TaxTotal = new ublXSD.TaxTotalType();
                            myInvoice.InvoiceLine[iDetay].TaxTotal.TaxAmount = new ublXSD.TaxAmountType();
                            myInvoice.InvoiceLine[iDetay].TaxTotal.TaxAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                            myInvoice.InvoiceLine[iDetay].TaxTotal.TaxAmount.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["kdvtutari"].ToString()).ToString("0.##"));//kdv tutari

                            bool btevkifat = false;
                            int sira = 0;
                            if (btevkifat == true)
                            {
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal = new ublXSD.TaxSubtotalType[2];//tevkifat
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira] = new ublXSD.TaxSubtotalType();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].CalculationSequenceNumeric = new ublXSD.CalculationSequenceNumericType();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].CalculationSequenceNumeric.Value = 1;
                                sira++;
                            }
                            else
                            {
                                if (btevkifat != true)
                                    myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal = new ublXSD.TaxSubtotalType[1];
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira] = new ublXSD.TaxSubtotalType();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].CalculationSequenceNumeric = new ublXSD.CalculationSequenceNumericType();
                                if (btevkifat != true)
                                {
                                    myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].CalculationSequenceNumeric.Value = 1;
                                }
                                else
                                {
                                    myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].CalculationSequenceNumeric.Value = 2;
                                }
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxableAmount = new ublXSD.TaxableAmountType();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxableAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxableAmount.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["kdvsiztutar"].ToString()).ToString("0.##"));//kdvsiztutar
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxAmount = new ublXSD.TaxAmountType();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxAmount.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["kdvtutari"].ToString()).ToString("0.##"));//kdvtutari
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].Percent = new ublXSD.PercentType();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].Percent.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["kdvorani"].ToString()).ToString("0.##"));//kdv orani
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxCategory = new ublXSD.TaxCategoryType();
                                if (double.Parse(dtinvdet.Rows[iDetay]["kdvtutari"].ToString()) == 0)
                                {
                                    //kdv istisnası
                                    bKdvIstisnasi = true;
                                    myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxCategory.TaxExemptionReason = new ublXSD.TaxExemptionReasonType();
                                    myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxCategory.TaxExemptionReason.Value = "Kdv İstisnası";
                                }
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxCategory.TaxScheme = new ublXSD.TaxSchemeType();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxCategory.TaxScheme.Name = new ublXSD.NameType1();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxCategory.TaxScheme.Name.Value = Conn.XmlEscape("KDV");
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxCategory.TaxScheme.TaxTypeCode = new ublXSD.TaxTypeCodeType();
                                myInvoice.InvoiceLine[iDetay].TaxTotal.TaxSubtotal[sira].TaxCategory.TaxScheme.TaxTypeCode.Value = "0015";
                            }

                            //iskonto
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge = new ublXSD.AllowanceChargeType();
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.ChargeIndicator = new ublXSD.ChargeIndicatorType();
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.ChargeIndicator.Value = false;//iskonto icin false artirim icin true
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.MultiplierFactorNumeric = new ublXSD.MultiplierFactorNumericType();
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.MultiplierFactorNumeric.Value = (decimal)double.Parse(dtinvdet.Rows[iDetay]["iskontoorani"].ToString());//iskonto orani
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.Amount = new ublXSD.AmountType1();
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.Amount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.Amount.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["kdvliisktutar"].ToString()).ToString("0.##"));//iskontotutari
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.BaseAmount = new ublXSD.BaseAmountType();
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.BaseAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                            myInvoice.InvoiceLine[iDetay].AllowanceCharge.BaseAmount.Value = (decimal)double.Parse(double.Parse(dtinvdet.Rows[iDetay]["kdvsizisktutar"].ToString()).ToString("0.##"));//kdvsiz tutar

                            itoplamSatir++;
                        }

                        myInvoice.LineCountNumeric = new ublXSD.LineCountNumericType();
                        myInvoice.LineCountNumeric.Value = itoplamSatir;

                        //toto: cac:InvoiceLine

                        //todo: cac:AllowanceCharge toplam iskonto
                        myInvoice.AllowanceCharge = new ublXSD.AllowanceChargeType();
                        myInvoice.AllowanceCharge.ChargeIndicator = new ublXSD.ChargeIndicatorType();
                        myInvoice.AllowanceCharge.ChargeIndicator.Value = false;//iskonto için false
                        myInvoice.AllowanceCharge.Amount = new ublXSD.AmountType1();
                        myInvoice.AllowanceCharge.Amount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                        myInvoice.AllowanceCharge.Amount.Value = (decimal)double.Parse(toplamkdvsiziskonto.ToString("0.##"));//toplam iskonto tutari
                        //toto: cac:TaxTotal
                        myInvoice.TaxTotal = new ublXSD.TaxTotalType[1];

                        myInvoice.TaxTotal[0] = new ublXSD.TaxTotalType();
                        myInvoice.TaxTotal[0].TaxAmount = new ublXSD.TaxAmountType();
                        myInvoice.TaxTotal[0].TaxAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                        myInvoice.TaxTotal[0].TaxAmount.Value = (decimal)double.Parse(toplamkdv.ToString("0.##"));
                        myInvoice.TaxTotal[0].TaxSubtotal = new ublXSD.TaxSubtotalType[kdvler.Count];
                        int ikdv = 0;
                        foreach (KDV kdv in kdvler)
                        {
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv] = new ublXSD.TaxSubtotalType();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxableAmount = new ublXSD.TaxableAmountType();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxableAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxableAmount.Value = (decimal)double.Parse(kdv.kdvMatrahi.ToString("0.##"));
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxAmount = new ublXSD.TaxAmountType();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxAmount.Value = (decimal)double.Parse(kdv.kdvTutari.ToString("0.##"));
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].CalculationSequenceNumeric = new ublXSD.CalculationSequenceNumericType();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].CalculationSequenceNumeric.Value = 1;
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].Percent = new ublXSD.PercentType();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].Percent.Value = (decimal)kdv.kdvOrani;
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxCategory = new ublXSD.TaxCategoryType();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxCategory.TaxScheme = new ublXSD.TaxSchemeType();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxCategory.TaxScheme.Name = new ublXSD.NameType1();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxCategory.TaxScheme.Name.Value = "KDV";
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxCategory.TaxScheme.TaxTypeCode = new ublXSD.TaxTypeCodeType();
                            myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxCategory.TaxScheme.TaxTypeCode.Value = "0015";
                            if (bKdvIstisnasi)
                            {
                                myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxCategory.TaxExemptionReason = new ublXSD.TaxExemptionReasonType();

                                myInvoice.TaxTotal[0].TaxSubtotal[ikdv].TaxCategory.TaxExemptionReason.Value = "Kdv İstisnası";
                            }


                            ikdv++;
                        }

                        double toplamdoviz = 0, toplamtl = 0;
                        if (dkondetdet != null && dkondetdet.Rows.Count > 0)
                        {
                            myInvoice.Note = new ublXSD.NoteType[2 + dkondetdet.Rows.Count];
                            if (bKdvIstisnasi)
                            {
                                kdvIstisnasiNotu = "* Iç hat bilet tutarlarnda Alan Vergisi hariç KDV dahildir. *Dış hat uçak biletleri KDV den muaftır.";
                                DialogResult dr = Conn.InputBox("İstisna Nedeni", "Lütfen KDV istisnası nedenini giriniz!", false, ref kdvIstisnasiNotu);
                                if (dr == DialogResult.OK)
                                {
                                    myInvoiceNotes += kdvIstisnasiNotu;
                                }
                            }
                            ikdv = 0;
                            foreach (DataRow r in dkondetdet.Rows)
                            {

                                myInvoice.Note[ikdv] = new ublXSD.NoteType();
                                myInvoice.Note[ikdv].Value = "Voucher:" + r["voucherno"].ToString() + " Name:" +
                                                             Conn.convert2UTF(r["adi"].ToString()) + " " +
                                                             Conn.convert2UTF(r["soyadi"].ToString()) +
                                                             " Arr:" + r["gelistarihi"].ToString() + " Dep:" + r["ayrilistarihi"].ToString() + " Detail:" +
                                                             Conn.convert2UTF(r["detay"].ToString());
                                toplamdoviz += double.Parse(r["toplamdovizoda"].ToString());
                                toplamtl += double.Parse(r["toplamtloda"].ToString());

                                ikdv++;
                            }
                            //todo doviz ve tl kontrolleri yapılacak
                            myInvoice.Note[ikdv] = new ublXSD.NoteType();
                            myInvoice.Note[ikdv].Value = "Toplamlar :" + toplamdoviz.ToString() + " " + toplamtl.ToString();
                            ikdv++;

                            myInvoice.Note[ikdv] = new ublXSD.NoteType();
                            myInvoice.Note[ikdv].Value = myInvoiceNotes;
                        }
                        else
                        {
                            myInvoice.Note = new ublXSD.NoteType[1];
                            myInvoice.Note[0] = new ublXSD.NoteType();
                            if (bKdvIstisnasi)
                                myInvoiceNotes += "* Iç hat bilet tutarlarnda Alan Vergisi hariç KDV dahildir. *Dış hat uçak biletleri KDV den muaftır.";
                            myInvoice.Note[0].Value = myInvoiceNotes;
                        }
                        //todo: cac:LegalMonetaryTotal
                        myInvoice.LegalMonetaryTotal = new ublXSD.MonetaryTotalType();
                        myInvoice.LegalMonetaryTotal.LineExtensionAmount = new ublXSD.LineExtensionAmountType();
                        myInvoice.LegalMonetaryTotal.LineExtensionAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                        myInvoice.LegalMonetaryTotal.LineExtensionAmount.Value = (decimal)double.Parse((toplamkdvsiz - toplamkdvsiziskonto).ToString("0.##"));//toplam kdvsiz tutar

                        myInvoice.LegalMonetaryTotal.TaxExclusiveAmount = new ublXSD.TaxExclusiveAmountType();
                        myInvoice.LegalMonetaryTotal.TaxExclusiveAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                        myInvoice.LegalMonetaryTotal.TaxExclusiveAmount.Value = (decimal)double.Parse(toplamkdvsiz.ToString("0.##"));//toplam kdvsiz tutar

                        myInvoice.LegalMonetaryTotal.TaxInclusiveAmount = new ublXSD.TaxInclusiveAmountType();
                        myInvoice.LegalMonetaryTotal.TaxInclusiveAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                        myInvoice.LegalMonetaryTotal.TaxInclusiveAmount.Value = (decimal)double.Parse((toplamkdvsiz + toplamkdv).ToString("0.##"));//kdv dahil tutar iskontosuz

                        myInvoice.LegalMonetaryTotal.AllowanceTotalAmount = new ublXSD.AllowanceTotalAmountType();
                        myInvoice.LegalMonetaryTotal.AllowanceTotalAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                        myInvoice.LegalMonetaryTotal.AllowanceTotalAmount.Value = (decimal)double.Parse(toplamkdvsiziskonto.ToString("0.##"));//toplam iskonto

                        myInvoice.LegalMonetaryTotal.PayableAmount = new ublXSD.PayableAmountType();
                        myInvoice.LegalMonetaryTotal.PayableAmount.currencyID = ublXSD.CurrencyCodeContentType.TRL;
                        myInvoice.LegalMonetaryTotal.PayableAmount.Value = (decimal)double.Parse(((toplamkdvsiz + toplamkdv) - toplamkdvsiziskonto).ToString("0.##"));//kdv dahil iskonto hariç ödenecek toplam tutar



                        if (System.IO.File.Exists(saveXmlfileNameTemp))
                        {
                            System.IO.File.Delete(saveXmlfileNameTemp);
                        }
                        if (System.IO.File.Exists(saveXmlfileNameTemp + "1"))
                        {
                            System.IO.File.Delete(saveXmlfileNameTemp + "1");
                        }

                        StreamWriter sw = new StreamWriter(saveXmlfileNameTemp + "1");

                        if (System.IO.File.Exists(saveXmlFolder + myInvoiceNumber + ".xslt"))
                        {
                            System.IO.File.Delete(saveXmlFolder + myInvoiceNumber + ".xslt");
                        }
                        System.IO.File.Copy(Conn.ROOT_DIR + "efatura\\efatura.xslt",
                                            saveXmlFolder + myInvoiceNumber + ".xslt");

                        mySerializer.Serialize(sw, myInvoice, ns);
                        sw.Close();

                        using (StreamReader sr = new StreamReader(saveXmlfileNameTemp + "1"))
                        {
                            int ln = 0;
                            using (var writer = new StreamWriter(saveXmlfileNameTemp))
                            {
                                string line = "";
                                writer.NewLine = "\r\n";
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (ln == 1)
                                    {
                                        writer.WriteLine("<Invoice xmlns:cac=\"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2\" " +
                                                         "xmlns:cbc=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\" " +
                                                         "xmlns:udt=\"urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2\" " +
                                                         "xmlns:ccts=\"urn:un:unece:uncefact:documentation:2\" " +
                                                         "xmlns:ext=\"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2\" " +
                                                         "xmlns:qdt=\"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2\" " +
                                                         "xmlns:ubltr=\"urn:oasis:names:specification:ubl:schema:xsd:TurkishCustomizationExtensionComponents\" " +
                                                         "xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\" " +
                                                         "xmlns:xades=\"http://uri.etsi.org/01903/v1.3.2#\" " +
                                                         "xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2\" " +
                                                         "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                                                         "xsi:schemaLocation=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2 UBLTR-Invoice-2.0.xsd\">"
                                                         );
                                    }
                                    else
                                    {
                                        writer.WriteLine(line);//.Trim());
                                    }
                                    ln++;
                                }
                                writer.Flush();
                                writer.Close();
                            }
                            sr.Close();
                        }

                        sql = String.Format("update {0} set efatura=2 where id={1}", invoiceFile, myInvoiceID);
                        Conn.ExecuteSql(sql);

                        sql = String.Format("update {0} set efatura=-3 where id={1}", invoiceFile, myInvoiceID);
                        Conn.ExecuteSql(sql);
                        bool imzaAt = false;
                        bool devamEdilsin = false;

                        if (Conn.INGEntVar)
                        {
                            if (pinNo != "")
                            {
                                DialogResult dri = MessageBox.Show("Mali Mühür İle İmzalama Yapılsınmı?", "Mali Mühür İmzalama", MessageBoxButtons.YesNo);
                                imzaAt = dri == DialogResult.Yes;
                            }
                            devamEdilsin = !imzaAt;
                        }
                        else
                        {
                            imzaAt = true;
                        }

                        if (imzaAt)
                        {
                            devamEdilsin = ublSigner.createEnvelopedBes(pinNo, saveXmlfileNameTemp, saveXmlfileName, true);
                        }

                        if (devamEdilsin)
                        {

                            if (Conn.INGEntVar && !imzaAt)
                            {
                                if (File.Exists(saveXmlfileName)) File.Delete(saveXmlfileName);
                                System.IO.File.Copy(saveXmlfileNameTemp, saveXmlfileName);
                            }

                            System.IO.File.Delete(saveXmlfileNameTemp);
                            System.IO.File.Delete(saveXmlfileNameTemp + "1");

                            System.IO.Directory.SetCurrentDirectory(saveXmlFolder);
                            System.IO.File.Copy(AppPath + (Conn.INGEntVar && !imzaAt ? "UBL-TR_ING_Schematron.xml" : "UBL-TR_Main_Schematron.xml"), saveXmlFolder + "sch.xml", true);
                            System.IO.File.Copy(AppPath + "probatron.jar", saveXmlFolder + "schematronvl.jar", true);


                            string args = "java -jar schematronvl.jar " + myInvoiceNumber + ".xml sch.xml > " + myInvoiceNumber + ".result.xml";
                            Conn.RunShell(args);

                            System.IO.Directory.SetCurrentDirectory(AppPath);
                            string schres = saveXmlFolder + myInvoiceNumber + ".result.xml";
                            string schtext = "";
                            if (System.IO.File.Exists(schres))
                            {
                                XmlDocument xr = new XmlDocument();
                                try
                                {
                                    xr.Load(schres);
                                    XmlElement x1 = (XmlElement)xr.GetElementsByTagName("svrl:schematron-output").Item(0);
                                    foreach (XmlElement e in x1.GetElementsByTagName("svrl:failed-assert"))
                                    {
                                        XmlElement x2 = (XmlElement)e.GetElementsByTagName("svrl:text").Item(0);
                                        schtext += x2.InnerText + "\r\n";
                                    }
                                }
                                finally
                                {
                                    xr = null;
                                }

                            }

                            if (!SchemaValidator.Validate(AppPath + "schema\\UBLTR-Invoice-2.0.xsd", saveXmlfileName))
                            {
                                schtext += "\r\nSchema Kontrolü Hatalı Detay:\r\n" + SchemaValidator.SchemaValidatorErrors;

                            }

                            if (schtext != "")
                            {

                                MessageBox.Show("Schematron hataları tespit edildi!\r\n" + schtext);
                                sql = String.Format("update {0} set efatura=1 where id={1}", invoiceFile, myInvoiceID);
                                Conn.ExecuteSql(sql);
                            }
                            else
                            {

                                sql = String.Format("update {0} set efatura=3 where id={1}", invoiceFile, myInvoiceID);
                                Conn.ExecuteSql(sql);


                                List<String> s = new List<String>();
                                s.Add(saveXmlfileName);

                                if (myCreateZipFile(saveXmlFolder + myTaxNumber + "_" + myInvoiceNumber + ".zip", s))
                                {

                                    //save signed xml into db
                                    byte[] rawXmlData = File.ReadAllBytes(saveXmlfileName);
                                    byte[] rawxsltData = File.ReadAllBytes(saveXmlFolder + myInvoiceNumber + ".xslt");
                                    byte[] rawZipData = File.ReadAllBytes(saveXmlFolder + myTaxNumber + "_" + myInvoiceNumber + ".zip");
                                    //ing ise esya api yok
                                    byte[] rawLogData;
                                    if (File.Exists(AppPath + "ESYA_API.log"))
                                    {
                                        rawLogData = File.ReadAllBytes(AppPath + "ESYA_API.log");
                                        File.Delete(AppPath + "ESYA_API.log");
                                    }
                                    else
                                    {
                                        rawLogData = new byte[11];
                                        rawLogData = Encoding.UTF8.GetBytes("İMZASIZ XML");
                                    }

                                    using (MySqlCommand command = new MySqlCommand())
                                    {
                                        string isql = "insert into efatura.efaturalog select *,null,null from efatura.efatura where faturaid='" + myInvoiceID + "' and faturatipi='" + myInvoiceType + "'";
                                        Conn.ExecuteSql(isql);

                                        isql = "delete from efatura.efatura where faturaid='" + myInvoiceID + "' and faturatipi='" + myInvoiceType + "'";
                                        Conn.ExecuteSql(isql);

                                        command.CommandText = "INSERT INTO efatura.efatura (faturaid, faturatipi, faturatarihi, faturaxml, faturaxslt, faturazip, faturadb, faturalog) VALUES " +
                                                              "('" + myInvoiceID + "','" + myInvoiceType + "','" + DateTime.Now.ToString("yyyy-MM-dd") +
                                                              "', ?faturaxml, ?faturaxslt, ?faturazip, '" + Conn.myConn.Database.ToString() + "', ?faturalog)";
                                        command.Connection = Conn.myConn;

                                        MySqlParameter faturaxmlParameter = new MySqlParameter("?faturaxml", MySqlDbType.Blob, rawXmlData.Length);
                                        MySqlParameter faturaxsltParameter = new MySqlParameter("?faturaxslt", MySqlDbType.Blob, rawxsltData.Length);
                                        MySqlParameter faturazipParameter = new MySqlParameter("?faturazip", MySqlDbType.Blob, rawZipData.Length);
                                        MySqlParameter faturalogParameter = new MySqlParameter("?faturalog", MySqlDbType.Blob, rawZipData.Length);

                                        faturaxmlParameter.Value = rawXmlData;
                                        faturaxsltParameter.Value = rawxsltData;
                                        faturazipParameter.Value = rawZipData;
                                        faturalogParameter.Value = rawLogData;

                                        command.Parameters.Add(faturaxmlParameter);
                                        command.Parameters.Add(faturaxsltParameter);
                                        command.Parameters.Add(faturazipParameter);
                                        command.Parameters.Add(faturalogParameter);
                                        command.ExecuteNonQuery();

                                        isql = "optimize table efatura.efatura";
                                        Conn.ExecuteSql(isql);
                                    }

                                }
                                string sxml = File.ReadAllText(saveXmlfileName, Encoding.UTF8);
                                StringBuilder sb = new StringBuilder(sxml);

                                sb.Insert(39, "<?xml-stylesheet type=\"text/xsl\" href=\"" + myInvoiceNumber + ".xslt\"?>\r\n");
                                File.WriteAllText(saveXmlfileNameTemp, sb.ToString(), Encoding.UTF8);
                                res = true;
                            }
                        }
                        else
                        {
                            sql = String.Format("update {0} set efatura=1 where id={1}", invoiceFile, myInvoiceID);
                            Conn.ExecuteSql(sql);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sql = String.Format("update {0} set efatura=1 where id={1}", invoiceFile, myInvoiceID);
                Conn.ExecuteSql(sql);
                MessageBox.Show("Fatura Olusturulurken Hata Olustu.\r\n" +
                                ex.Message.ToString());
            }
            finally
            {
                Conn.CloseConn();
                mySettings = null;
                GC.Collect();
            }
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = txtImzalanacakSql.Text;
            string sbastar = dpBasTarih.Value.ToString("yyyy-MM-dd");
            string sbittar = dpBitTarih.Value.ToString("yyyy-MM-dd");
            sql = sql.Replace("/*sorguco*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            sql = sql.Replace("/*sorguaf*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            sql = sql.Replace("/*sorgucf*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            sql = sql.Replace("/*sorgusi*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            sql = sql.Replace("/*sorguag*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            Conn.OpenConn();
            da = Conn.OpenAdapter(sql);
            cb = new MySql.Data.MySqlClient.MySqlCommandBuilder(da);
            dt = new DataTable();
            da.Fill(dt);

            dgView.DataSource = dt;
            dgView.ContextMenuStrip = pmImzalanacaklar;
            dgView.AutoResizeColumns();
            Conn.CloseConn();
        }

        private void frmInvoice_Load(object sender, EventArgs e)
        {
            this.Text = "Fatura İmzalama İşlemleri (" + Conn.aliasName + ")";
            string yil = Conn.Right(Properties.Settings.Default.MysqlDB, 4);
            dpBasTarih.Value = DateTime.Parse(yil + "-01-01");
            dpBasTarih.Update();
            dpBitTarih.Value = DateTime.Now.Date;

            try
            {
                Conn.OpenConn();
                myTaxNumber = Conn.SabitOku("OtelVergiNo");
                myAddressRoom = Conn.SabitOku("eFaturaRoom");
                myAddressStreet = Conn.SabitOku("eFaturaStreetName");
                myAddressRegion = Conn.SabitOku("eFaturaRegion");
                myAddressPostalZone = Conn.SabitOku("eFaturaPostalZone");
                myAddressCountry = Conn.SabitOku("eFaturaUlke");
                myAddressCitySubDivision = Conn.SabitOku("eFaturaCitySubDivisionName");
                myAddressCityName = Conn.SabitOku("eFaturaCityName");
                myAddressBuildingNumber = Conn.SabitOku("eFaturaBuildingNumber");
                myAddressBuildingName = Conn.SabitOku("eFaturaBuildingName");
                myCompanyName = Conn.SabitOku("eFaturaUnvan");
                myEmail = Conn.SabitOku("eFaturaEmail");
                myFax = Conn.SabitOku("eFaturaFax");
                myInvoiceNotes = Conn.SabitOku("eFaturaNotlar");
                myTelephone = Conn.SabitOku("eFaturaTelefon");
                myTaxOffice = Conn.SabitOku("OtelVergiDairesi");
                myTradeNumber = Conn.SabitOku("eFaturaTicaretSicilNo");
                myMersisNumber = Conn.SabitOku("eFaturaMersisno");
                myWeb = Conn.SabitOku("eFaturaWeb");
            }
            finally
            {
                Conn.CloseConn();
            }
            btnIngKayitliKullaniciListesi.Enabled = Conn.INGEntVar;
            btnIngKayitliKullaniciListesi.Visible = Conn.INGEntVar;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int i;
            pinNo = txtPinKodu.Text.Trim();
            imzalananFaturalar.Clear();
            if (Conn.INGEntVar == false)
            {
                if (pinNo == "")
                {
                    MessageBox.Show("Lütfen Mali Mühürünüzün Pin 'ini giriniz!", "Lütfen Pini Giriniz");
                    return;
                }
            }

            pgImzalama.Maximum = dgView.RowCount;
            pgImzalama.Value = 0;
            pgImzalama.Step = 1;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();

                for (i = 0; i < dgView.RowCount; i++)
                {

                    if (dgView.Rows[i].Cells["sec"].Value.ToString() == "1")
                    {
                        string myinvoiceType = "", myinvoceID = "", myInvoiceNumber = "";
                        myinvoiceType = dgView.Rows[i].Cells["tip"].Value.ToString();
                        myinvoceID = dgView.Rows[i].Cells["id"].Value.ToString();
                        if (CreateUBL(myinvoceID, myinvoiceType, ref myInvoiceNumber))
                        {
                            dgView.Rows[i].Cells["sec"].Value = "2";
                            imzalananFaturalar.Add(myInvoiceNumber);

                        }
                    }
                    pgImzalama.PerformStep();
                    this.Refresh();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnListUnSigned.PerformClick();
            }

            if (imzalananFaturalar.Count > 1 && !Conn.INGEntVar)
            {
                DialogResult dr = MessageBox.Show("İmzalanan Faturalar tek bir paket haline getirilsin mi?", "Paket Oluşturma", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    Properties.Settings mySettings = new Properties.Settings();
                    string saveXmlFolder = Conn.SaveXmlPath;
                    Guid g = Guid.NewGuid();
                    List<String> s = new List<String>();
                    foreach (string sfatno in imzalananFaturalar)
                    {
                        s.Add(saveXmlFolder + sfatno + ".xml");
                    }
                    myCreateZipFile(saveXmlFolder + g.ToString() + ".zip", s);
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                dgView.Rows[i].Cells["sec"].Value = "1";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            string sql = txtImzalananSql.Text;
            string sbastar = dpBasTarih.Value.ToString("yyyy-MM-dd");
            string sbittar = dpBitTarih.Value.ToString("yyyy-MM-dd");
            sql = sql.Replace("/*sorguco*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            sql = sql.Replace("/*sorguaf*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            sql = sql.Replace("/*sorgucf*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            sql = sql.Replace("/*sorgusi*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            sql = sql.Replace("/*sorguag*/", String.Format(" and tarih between '{0}' and '{1}'", sbastar, sbittar));
            Conn.OpenConn();
            da = Conn.OpenAdapter(sql);
            cb = new MySql.Data.MySqlClient.MySqlCommandBuilder(da);
            dt = new DataTable();
            da.Fill(dt);
            dgView.DataSource = dt;
            dgView.ContextMenuStrip = pmImzalanlar;
            dgView.AutoResizeColumns();
            Conn.CloseConn();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Properties.Settings mySettings = new Properties.Settings();
            string saveXmlFolder = System.IO.Path.GetTempPath();

            FileStream stream;
            BinaryWriter writer;
            // Size of the BLOB buffer.
            int bufferSize = 100;

            // The BLOB byte[] buffer to be filled by GetBytes.
            byte[] outByte = new byte[bufferSize];

            // The bytes returned from GetBytes.
            long retval;

            // The starting position in the BLOB output.
            long startIndex = 0;

            Conn.OpenConn();
            try
            {
                foreach (DataGridViewRow row in dgView.SelectedRows)
                {
                    string sql = "", saveXmlfileName = "", faturaTipi, faturaID;
                    saveXmlfileName = row.Cells["faturano"].Value.ToString();
                    faturaID = row.Cells["id"].Value.ToString();
                    faturaTipi = row.Cells["tip"].Value.ToString();
                    sql = "select * from efatura.efatura where faturatipi='" + faturaTipi + "' and faturaid='" + faturaID + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, Conn.myConn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();
                    if (File.Exists(saveXmlFolder + saveXmlfileName + ".xml"))
                    {
                        File.Delete(saveXmlFolder + saveXmlfileName + ".xml");
                    }

                    stream = new FileStream(saveXmlFolder + saveXmlfileName + ".xml", FileMode.OpenOrCreate, FileAccess.Write);
                    writer = new BinaryWriter(stream);

                    // Reset the starting byte for the new BLOB.
                    startIndex = 0;

                    // Read bytes into outByte[] and retain the number of bytes returned.
                    retval = reader.GetBytes(reader.GetOrdinal("faturaxml"), startIndex, outByte, 0, bufferSize);

                    // Continue while there are bytes beyond the size of the buffer.
                    while (retval == bufferSize)
                    {
                        writer.Write(outByte);
                        writer.Flush();
                        try
                        {

                            // Reposition start index to end of last buffer and fill buffer.
                            startIndex += bufferSize;
                            retval = reader.GetBytes(reader.GetOrdinal("faturaxml"), startIndex, outByte, 0, bufferSize);
                        }
                        catch (Exception ex)
                        {
                            retval = 0;
                        }
                    }

                    // Write the remaining buffer.
                    writer.Write(outByte, 0, (int)retval);
                    writer.Flush();

                    // Close the output file.
                    writer.Close();
                    stream.Close();


                    if (File.Exists(saveXmlFolder + saveXmlfileName + ".xslt"))
                    {
                        File.Delete(saveXmlFolder + saveXmlfileName + ".xslt");
                    }
                    stream = new FileStream(saveXmlFolder + saveXmlfileName + ".xslt", FileMode.OpenOrCreate, FileAccess.Write);
                    writer = new BinaryWriter(stream);

                    // Reset the starting byte for the new BLOB.
                    startIndex = 0;

                    // Read bytes into outByte[] and retain the number of bytes returned.
                    retval = reader.GetBytes(reader.GetOrdinal("faturaxslt"), startIndex, outByte, 0, bufferSize);

                    // Continue while there are bytes beyond the size of the buffer.
                    while (retval == bufferSize)
                    {
                        writer.Write(outByte);
                        writer.Flush();

                        try
                        {
                            // Reposition start index to end of last buffer and fill buffer.
                            startIndex += bufferSize;
                            retval = reader.GetBytes(reader.GetOrdinal("faturaxslt"), startIndex, outByte, 0, bufferSize);
                        }
                        catch (Exception exp)
                        {
                            retval = 0;
                        }
                    }

                    // Write the remaining buffer.
                    writer.Write(outByte, 0, (int)retval);
                    writer.Flush();

                    // Close the output file.
                    writer.Close();
                    stream.Close();


                    string sxml = File.ReadAllText(saveXmlFolder + saveXmlfileName + ".xml", Encoding.UTF8);
                    StringBuilder sb = new StringBuilder(sxml);
                    sb.Insert(39, "<?xml-stylesheet type=\"text/xsl\" href=\"" + saveXmlfileName + ".xslt\"?>\r\n");
                    File.WriteAllText(saveXmlFolder + saveXmlfileName + ".xml", sb.ToString(), Encoding.UTF8);
                    frmInvoiceViewer.ShowInvoice(saveXmlFolder + saveXmlfileName + ".xml");
                    reader.Close();
                    cmd = null;
                    reader = null;
                }
            }
            finally
            {

                Conn.CloseConn();
            }

        }

        private void yazdırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnPrint.PerformClick();
        }

        private void imzalamayıGeriAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conn.OpenConn();
            try
            {
                foreach (DataGridViewRow row in dgView.SelectedRows)
                {

                    string sql = "", myInvoiceType, myInvoiceID, myInvoiceNumber, invoiceFile = "";
                    myInvoiceNumber = row.Cells["faturano"].Value.ToString();
                    myInvoiceID = row.Cells["id"].Value.ToString();
                    myInvoiceType = row.Cells["tip"].Value.ToString();
                    switch (myInvoiceType)
                    {
                        case "CO":
                            invoiceFile = "checkoutfatura";
                            break;
                        case "AF":
                            invoiceFile = "acentafatura";
                            break;
                        case "SF":
                            invoiceFile = "sifatura";
                            break;
                        case "CF":
                            invoiceFile = "carifatura";
                            break;
                        case "AG":
                            invoiceFile = "adisyonfaturasi";
                            break;
                    }
                    //todo+:fatura iletildi ise izin verme
                    int efaturadurumu = 0;
                    sql = string.Format("select cast(efatura as char) from {0} where id={1}", invoiceFile, myInvoiceID);
                    try
                    {
                        efaturadurumu = int.Parse(Conn.ReadSingleField(sql, "0"));
                    }
                    catch (Exception exp)
                    {

                    }
                    //todo-:red edilen faturalar loglanarak yeniden düzenlenmesine izin verilmeli
                    if ((efaturadurumu > 0 && efaturadurumu < 4))
                    {
                        sql = string.Format("update {0} set efatura=1 where id={1}", invoiceFile, myInvoiceID);
                        Conn.ExecuteSql(sql);
                    }
                }
                btnListSigned.PerformClick();
            }
            finally
            {

                Conn.CloseConn();
            }
        }

        private void gönderildiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conn.OpenConn();
            try
            {
                foreach (DataGridViewRow row in dgView.SelectedRows)
                {
                    string sql = "", myInvoiceType, myInvoiceID, myInvoiceNumber, invoiceFile = "";
                    myInvoiceNumber = row.Cells["faturano"].Value.ToString();
                    myInvoiceID = row.Cells["id"].Value.ToString();
                    myInvoiceType = row.Cells["tip"].Value.ToString();
                    switch (myInvoiceType)
                    {
                        case "CO":
                            invoiceFile = "checkoutfatura";
                            break;
                        case "AF":
                            invoiceFile = "acentafatura";
                            break;
                        case "SF":
                            invoiceFile = "sifatura";
                            break;
                        case "CF":
                            invoiceFile = "carifatura";
                            break;
                        case "AG":
                            invoiceFile = "adisyonfaturasi";
                            break;
                    }
                    sql = string.Format("update {0} set efatura=4 where id={1}", invoiceFile, myInvoiceID);
                    Conn.ExecuteSql(sql);
                }
                btnListSigned.PerformClick();
            }
            finally
            {

                Conn.CloseConn();
            }
        }

        private void kabulEdildiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conn.OpenConn();
            try
            {
                foreach (DataGridViewRow row in dgView.SelectedRows)
                {
                    string sql = "", myInvoiceType, myInvoiceID, myInvoiceNumber, invoiceFile = "";
                    myInvoiceNumber = row.Cells["faturano"].Value.ToString();
                    myInvoiceID = row.Cells["id"].Value.ToString();
                    myInvoiceType = row.Cells["tip"].Value.ToString();
                    switch (myInvoiceType)
                    {
                        case "CO":
                            invoiceFile = "checkoutfatura";
                            break;
                        case "AF":
                            invoiceFile = "acentafatura";
                            break;
                        case "SF":
                            invoiceFile = "sifatura";
                            break;
                        case "CF":
                            invoiceFile = "carifatura";
                            break;
                        case "AG":
                            invoiceFile = "adisyonfaturasi";
                            break;
                    }
                    sql = string.Format("update {0} set efatura=5 where id={1}", invoiceFile, myInvoiceID);
                    Conn.ExecuteSql(sql);
                }
                btnListSigned.PerformClick();
            }
            finally
            {

                Conn.CloseConn();
            }
        }

        private void redEdildiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conn.OpenConn();
            try
            {
                foreach (DataGridViewRow row in dgView.SelectedRows)
                {
                    string sql = "", myInvoiceType, myInvoiceID, myInvoiceNumber, invoiceFile = "", myProfile = "";
                    myInvoiceNumber = row.Cells["faturano"].Value.ToString();
                    myInvoiceID = row.Cells["id"].Value.ToString();
                    myInvoiceType = row.Cells["tip"].Value.ToString();
                    myProfile = row.Cells["efaturasekli"].ToString();
                    if (myProfile == Conn.convert2Latin("TİCARİFATURA"))
                    {
                        switch (myInvoiceType)
                        {
                            case "CO":
                                invoiceFile = "checkoutfatura";
                                break;
                            case "AF":
                                invoiceFile = "acentafatura";
                                break;
                            case "SF":
                                invoiceFile = "sifatura";
                                break;
                            case "CF":
                                invoiceFile = "carifatura";
                                break;
                            case "AG":
                                invoiceFile = "adisyonfaturasi";
                                break;
                        }
                        sql = string.Format("update {0} set efatura=-3 where id={1}", invoiceFile, myInvoiceID);
                        Conn.ExecuteSql(sql);
                    }
                }
                btnListSigned.PerformClick();
            }
            finally
            {

                Conn.CloseConn();
            }
        }

        private void faturaNoDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conn.OpenConn();
            try
            {
                foreach (DataGridViewRow row in dgView.SelectedRows)
                {
                    string sql, myInvoiceType, myInvoiceID, myInvoiceNumber, invoiceFile, myNewFaturaNumber, konaklamaAlan = "",
                           fisnofield = "", tarihfield = "";
                    myInvoiceNumber = row.Cells["faturano"].Value.ToString();
                    myInvoiceID = row.Cells["id"].Value.ToString();
                    myInvoiceType = row.Cells["tip"].Value.ToString();
                    invoiceFile = "";
                    fisnofield = "fisno";
                    tarihfield = "tarih";
                    switch (myInvoiceType)
                    {
                        case "CO":
                            invoiceFile = "checkoutfatura";
                            konaklamaAlan = "FATURANO";
                            fisnofield = "YEVMIYEFISNO";
                            break;
                        case "AF":
                            invoiceFile = "acentafatura";
                            konaklamaAlan = "ACENTAFATURANO";
                            break;
                        case "SF":
                            invoiceFile = "sifatura";
                            break;
                        case "CF":
                            invoiceFile = "carifatura";
                            break;
                        case "AG":
                            invoiceFile = "adisyonfaturasi";
                            break;
                    }
                    /*todo
                     * +:muhasebe fiş açıklaması replace
                     * +:ch fiş açıklaması replace
                     * +:c/o ise ayrilankonaklamabilgileri faturano change
                     * -:işlem yavaş ilerliyor
                     */
                    if (invoiceFile != "")
                    {
                        myNewFaturaNumber = Conn.Right(myInvoiceNumber, myInvoiceNumber.Length - 7);

                        DialogResult dr = Conn.InputBox("Yeni Faturano Giriniz", "Lütfen Yeni Fatura Numarasını Giriniz!", false, ref myNewFaturaNumber);
                        if (dr == DialogResult.OK)
                        {
                            string oldInvoiceNumber = myInvoiceNumber;
                            myInvoiceNumber = Conn.Left(myInvoiceNumber, 7) + myNewFaturaNumber;
                            if (konaklamaAlan != "")
                            {
                                sql = string.Format("update {0} set {1}='{2}'", "ayrilankonaklamabilgileri", konaklamaAlan, myInvoiceNumber);
                                Conn.ExecuteSql(sql);
                            }

                            string mhtable = "", muhfisno = "", chtable = "";
                            DateTime fatTarih;

                            sql = string.Format("select {0} from {1} where id='{2}'", fisnofield, invoiceFile, myInvoiceID);
                            muhfisno = Conn.ReadSingleField(sql, fisnofield);

                            sql = string.Format("select {0} from {1} where id='{2}'", tarihfield, invoiceFile, myInvoiceID);
                            fatTarih = DateTime.Parse(Conn.ReadSingleField(sql, tarihfield));
                            mhtable = "mh" + fatTarih.ToString("MM");
                            chtable = "ch" + fatTarih.ToString("MM");

                            sql = string.Format("update {0} set aciklama=replace(aciklama,'{1}','{2}'), " +
                                                "FISGENELACIKLAMA=replace(FISGENELACIKLAMA,'{1}','{2}') " +
                                                "where fisno='{3}'", mhtable, oldInvoiceNumber, myInvoiceNumber, muhfisno);
                            Conn.ExecuteSql(sql);

                            sql = string.Format("update {0} set aciklama=replace(aciklama,'{1}','{2}') " +
                                                "where fisno='{3}'", chtable, oldInvoiceNumber, myInvoiceNumber, muhfisno);
                            Conn.ExecuteSql(sql);

                            sql = string.Format("update {0} set faturano='{1}' where id={2}", invoiceFile, myInvoiceNumber, myInvoiceID);
                            Conn.ExecuteSql(sql);
                        }
                    }
                }
                btnListUnSigned.PerformClick();
            }
            finally
            {

                Conn.CloseConn();
            }
        }

        private void dgView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private string CreateEnvolope(string invoiceType, string invoiceID, string invoiceNumber)
        {
            string res = "";
            try
            {
                string saveXmlFolder = System.IO.Path.GetTempPath();
                string sql = "";
                sql = "select * from efatura.efatura where faturatipi='" + invoiceType + "' and faturaid='" + invoiceID + "'";

                FileStream stream;
                BinaryWriter writer;
                // Size of the BLOB buffer.
                int bufferSize = 100;

                // The BLOB byte[] buffer to be filled by GetBytes.
                byte[] outByte = new byte[bufferSize];

                // The bytes returned from GetBytes.
                long retval;

                // The starting position in the BLOB output.
                long startIndex = 0;

                MySqlCommand cmd = new MySqlCommand(sql, Conn.myConn);
                MySqlDataReader reader = cmd.ExecuteReader();

                reader.Read();
                if (File.Exists(saveXmlFolder + invoiceNumber + "XML.xml"))
                {
                    File.Delete(saveXmlFolder + invoiceNumber + "XML.xml");
                }

                stream = new FileStream(saveXmlFolder + invoiceNumber + "XML.xml", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new BinaryWriter(stream);

                // Reset the starting byte for the new BLOB.
                startIndex = 0;

                // Read bytes into outByte[] and retain the number of bytes returned.
                retval = reader.GetBytes(reader.GetOrdinal("faturaxml"), startIndex, outByte, 0, bufferSize);

                // Continue while there are bytes beyond the size of the buffer.
                while (retval == bufferSize)
                {
                    writer.Write(outByte);
                    writer.Flush();
                    try
                    {

                        // Reposition start index to end of last buffer and fill buffer.
                        startIndex += bufferSize;
                        retval = reader.GetBytes(reader.GetOrdinal("faturaxml"), startIndex, outByte, 0, bufferSize);
                    }
                    catch (Exception ex)
                    {
                        retval = 0;
                    }
                }

                // Write the remaining buffer.
                writer.Write(outByte, 0, (int)retval);
                writer.Flush();

                // Close the output file.
                writer.Close();
                stream.Close();

                reader.Close();
                cmd = null;
                reader = null;

                int ln = 0;
                using (StreamReader sr = new StreamReader(saveXmlFolder + invoiceNumber + "XML.xml"))
                {
                    using (var stw = new StreamWriter(saveXmlFolder + invoiceNumber + "XML.xml1"))
                    {
                        string line = "";
                        stw.NewLine = "\r\n";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (ln != 0)
                            {
                                stw.WriteLine(line);//.Trim());
                            }
                            ln++;
                        }
                        stw.Flush();
                        stw.Close();
                    }
                    sr.Close();
                }

                File.Delete(saveXmlFolder + invoiceNumber + "XML.xml");
                File.Copy(saveXmlFolder + invoiceNumber + "XML.xml1", saveXmlFolder + invoiceNumber + "XML.xml");
                File.Delete(saveXmlFolder + invoiceNumber + "XML.xml1");

                string sxml = File.ReadAllText(saveXmlFolder + invoiceNumber + "XML.xml", Encoding.UTF8);
                File.Delete(saveXmlFolder + invoiceNumber + "XML.xml");

                //zarf oluşturulacak
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("sh", "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader");
                ns.Add("ef", "http://www.efatura.gov.tr/package-namespace");
                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                XmlSerializer xs = new XmlSerializer(typeof(UBLEnvelope.StandardBusinessDocument));

                UBLEnvelope.StandardBusinessDocument sbd = new UBLEnvelope.StandardBusinessDocument();
                sbd.StandardBusinessDocumentHeader = new UBLEnvelope.StandardBusinessDocumentHeader();
                sbd.StandardBusinessDocumentHeader.HeaderVersion = "1.0";
                sbd.StandardBusinessDocumentHeader.Sender = new UBLEnvelope.Partner[1];
                sbd.StandardBusinessDocumentHeader.Sender[0] = new UBLEnvelope.Partner();
                sbd.StandardBusinessDocumentHeader.Sender[0].Identifier = new UBLEnvelope.PartnerIdentification();
                sbd.StandardBusinessDocumentHeader.Sender[0].Identifier.Value = Conn.INGAlias;
                sbd.StandardBusinessDocumentHeader.Sender[0].ContactInformation = new UBLEnvelope.ContactInformation[2];
                sbd.StandardBusinessDocumentHeader.Sender[0].ContactInformation[0] = new UBLEnvelope.ContactInformation();
                sbd.StandardBusinessDocumentHeader.Sender[0].ContactInformation[0].ContactTypeIdentifier = "UNVAN";
                sbd.StandardBusinessDocumentHeader.Sender[0].ContactInformation[0].Contact = myCompanyName;
                sbd.StandardBusinessDocumentHeader.Sender[0].ContactInformation[1] = new UBLEnvelope.ContactInformation();
                sbd.StandardBusinessDocumentHeader.Sender[0].ContactInformation[1].ContactTypeIdentifier = "VKN_TCKN";
                sbd.StandardBusinessDocumentHeader.Sender[0].ContactInformation[1].Contact = myTaxNumber;

                //select efaturaeposta from efaturabilgi where faturatipi='' and faturaid''
                sql = "select efaturaeposta, efaturaunvan, efaturatcknvkn from efaturabilgi where faturatipi='" + invoiceType + "' and faturaid='" + invoiceID + "'";
                string aliciAlias = Conn.ReadSingleField(sql, "0");
                string aliciUnvan = Conn.ReadSingleField(sql, "1");
                string aliciVKN = Conn.ReadSingleField(sql, "2");
                sbd.StandardBusinessDocumentHeader.Receiver = new UBLEnvelope.Partner[1];
                sbd.StandardBusinessDocumentHeader.Receiver[0] = new UBLEnvelope.Partner();
                sbd.StandardBusinessDocumentHeader.Receiver[0].Identifier = new UBLEnvelope.PartnerIdentification();
                sbd.StandardBusinessDocumentHeader.Receiver[0].Identifier.Value = aliciAlias;
                sbd.StandardBusinessDocumentHeader.Receiver[0].ContactInformation = new UBLEnvelope.ContactInformation[2];
                sbd.StandardBusinessDocumentHeader.Receiver[0].ContactInformation[0] = new UBLEnvelope.ContactInformation();
                sbd.StandardBusinessDocumentHeader.Receiver[0].ContactInformation[0].ContactTypeIdentifier = "UNVAN";
                sbd.StandardBusinessDocumentHeader.Receiver[0].ContactInformation[0].Contact = aliciUnvan;
                sbd.StandardBusinessDocumentHeader.Receiver[0].ContactInformation[1] = new UBLEnvelope.ContactInformation();
                sbd.StandardBusinessDocumentHeader.Receiver[0].ContactInformation[1].ContactTypeIdentifier = "VKN_TCKN";
                sbd.StandardBusinessDocumentHeader.Receiver[0].ContactInformation[1].Contact = aliciVKN;

                sbd.StandardBusinessDocumentHeader.DocumentIdentification = new UBLEnvelope.DocumentIdentification();
                sbd.StandardBusinessDocumentHeader.DocumentIdentification.Standard = "UBLTR";
                sbd.StandardBusinessDocumentHeader.DocumentIdentification.TypeVersion = "1.0";
                sbd.StandardBusinessDocumentHeader.DocumentIdentification.Type = "SENDERENVELOPE";
                sbd.StandardBusinessDocumentHeader.DocumentIdentification.CreationDateAndTime = DateTime.Now;

                Guid g = Guid.NewGuid();
                res = g.ToString();
                sbd.StandardBusinessDocumentHeader.DocumentIdentification.InstanceIdentifier = g.ToString();
                sbd.StandardBusinessDocumentHeader.Manifest = new UBLEnvelope.Manifest();
                sbd.StandardBusinessDocumentHeader.Manifest.NumberOfItems = "1";
                sbd.StandardBusinessDocumentHeader.Manifest.ManifestItem = new UBLEnvelope.ManifestItem[1];
                sbd.StandardBusinessDocumentHeader.Manifest.ManifestItem[0] = new UBLEnvelope.ManifestItem();
                sbd.StandardBusinessDocumentHeader.Manifest.ManifestItem[0].MimeTypeQualifierCode = "application/xml";
                sbd.StandardBusinessDocumentHeader.Manifest.ManifestItem[0].UniformResourceIdentifier = "";
                sbd.StandardBusinessDocumentHeader.Manifest.ManifestItem[0].Description = "";
                sbd.StandardBusinessDocumentHeader.Manifest.ManifestItem[0].LanguageCode = "TR";

                XmlSerializer pkgser = new XmlSerializer(typeof(UBLPkg.Package));
                //XmlNode pkgNode = new XmlDocument().CreateNode(XmlNodeType.Element, "ef:Package", "http://www.efatura.gov.tr/package-namespace");
                UBLPkg.Package pkg = new UBLPkg.Package();
                pkg.Elements = new UBLPkg.PackageElements[1];
                pkg.Elements[0] = new UBLPkg.PackageElements();
                pkg.Elements[0].ElementCount = 1;
                pkg.Elements[0].ElementType = "INVOICE";
                pkg.Elements[0].ElementList = new UBLPkg.PackageElementsElementList();

                XmlDocument x1 = new XmlDocument();
                x1.LoadXml(sxml);

                pkg.Elements[0].ElementList.Any = new XmlElement[1];
                pkg.Elements[0].ElementList.Any[0] = x1.DocumentElement;

                //pkgNode.InnerText = "";
                Stream st = new MemoryStream();
                TextWriter wrt = new StreamWriter(st);
                pkgser.Serialize(wrt, pkg);

                //Debug.WriteLine(wrt.ToString());



                XmlDocument stxml = new XmlDocument();
                st.Position = 0;
                stxml.Load(st);


                XmlDocument dom = new XmlDocument();
                XmlElement el = dom.CreateElement("ef", "package", "http://www.efatura.gov.tr/package-namespace");

                //var sr = new StreamReader(st);
                //var myStr = sr.ReadToEnd();
                el.InnerXml = stxml.DocumentElement.InnerXml;
                sbd.Any = el;

                if (System.IO.File.Exists(saveXmlFolder + res + ".xml"))
                {
                    System.IO.File.Delete(saveXmlFolder + res + ".xml");
                }

                StreamWriter sw = new StreamWriter(saveXmlFolder + res + ".xml");

                xs.Serialize(sw, sbd, ns);
                sw.Close();

                ln = 0;
                using (StreamReader sr = new StreamReader(saveXmlFolder + res + ".xml"))
                {
                    using (var stw = new StreamWriter(saveXmlFolder + res + ".xml1"))
                    {
                        string line = "";
                        stw.NewLine = "\r\n";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (ln == 1)
                            {
                                stw.WriteLine("<sh:StandardBusinessDocument " +
                                                 "xsi:schemaLocation=\"http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader PackageProxy.xsd\" " +
                                                 "xmlns:sh=\"http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader\" " +
                                                 "xmlns:ef=\"http://www.efatura.gov.tr/package-namespace\" " +
                                                 "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
                                                 );
                            }
                            else
                            {
                                stw.WriteLine(line);//.Trim());
                            }
                            ln++;
                        }
                        stw.Flush();
                        stw.Close();
                    }
                    sr.Close();
                }
                System.IO.File.Delete(saveXmlFolder + res + ".xml");
                System.IO.File.Copy(saveXmlFolder + res + ".xml1", saveXmlFolder + res + ".xml");
                System.IO.File.Delete(saveXmlFolder + res + ".xml1");

                if (!SchemaValidator.ValidateEnvolope(AppPath + "Envelope\\PackageProxy.xsd", saveXmlFolder + res + ".xml"))
                {
                    MessageBox.Show("Schema Hata Oluştu.\r\n" + SchemaValidator.SchemaValidatorErrors);
                    res = "";
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata Oluştu.\r\n" + ex.Message);
                res = "";
            }
            return res;
        }

        private void miINGGonder_Click(object sender, EventArgs e)
        {
            string HataMesaji = "";
            string saveXmlFolder = System.IO.Path.GetTempPath();
            string myInvoiceType = "", myInvoiceID = "", myInvoiceNumber = "", envUUID = "";
            try
            {
                Conn.OpenConn();


                INGUBL.EFaturaPortTypeClient cl = new INGUBL.EFaturaPortTypeClient();
                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)cl.InnerChannel))
                {
                    string auth = string.Format("{0}:{1}", Conn.INGUserName, Conn.INGPassword);
                    byte[] bytarr = System.Text.Encoding.ASCII.GetBytes(auth);
                    string base64auth = Convert.ToBase64String(bytarr);
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add(HttpRequestHeader.Authorization, String.Format("Basic {0}", base64auth));
                    foreach (DataGridViewRow row in dgView.SelectedRows)
                    {

                        myInvoiceNumber = row.Cells["faturano"].Value.ToString();
                        myInvoiceID = row.Cells["id"].Value.ToString();
                        myInvoiceType = row.Cells["tip"].Value.ToString();
                        envUUID = CreateEnvolope(myInvoiceType, myInvoiceID, myInvoiceNumber);
                        if (envUUID != "")
                        {

                            List<string> l = new List<string>();
                            l.Add(saveXmlFolder + envUUID + ".xml");
                            myCreateZipFile(saveXmlFolder + envUUID + ".zip", l);
                            byte[] bs = File.ReadAllBytes(saveXmlFolder + envUUID + ".zip");

                            string bshash = Conn.CalculateMD5HashBytes(bs);

                            INGUBL.documentReturnType dr;
                            INGUBL.documentType dt = new INGUBL.documentType()
                                                   {
                                                       fileName = envUUID + ".zip",
                                                       hash = bshash,
                                                       binaryData = new INGUBL.base64Binary()
                                                                    {
                                                                        Value = bs,
                                                                        contentType = "application/zip"
                                                                    }
                                                   };

                            dr = cl.sendDocument(dt);
                            MessageBox.Show(dr.msg.ToString());
                        }
                    }
                }
            }
            catch (FaultException<INGUBL.EFaturaFaultType> fault) //Catches INGeF Processing Exceptions
            {
                HataMesaji = string.Format("ProcessingFault. Message: {0}    Reason: {1}   Error Code :{2}    Detail:{3}", fault.Message, fault.Reason, fault.Detail.code, fault.Detail.msg);

                if (fault.InnerException != null)
                    HataMesaji += string.Format(" InnerExeption : {0}", fault.InnerException.Message);

            }
            catch (System.ServiceModel.FaultException fault) //Catches SOAP Exceptions
            {
                HataMesaji = string.Format("FaultException. Message: {0}    Reason: {1}   ", fault.Message, fault.Reason);

                if (fault.InnerException != null)
                    HataMesaji += string.Format(" InnerExeption : {0}", fault.InnerException.Message);
            }
            catch (HttpListenerException ex) //HttpListenerException, Unuthorized, Internet Server Error
            {
                HataMesaji = string.Format("HttpListenerException. Message: {0}  ErrorCode: {1}   NativeErrorCode:{2}   ", ex.Message, ex.ErrorCode, ex.NativeErrorCode);

                if (ex.InnerException != null)
                    HataMesaji += string.Format(" InnerExeption : {0}", ex.InnerException.Message);
            }
            catch (Exception ex) //Catches all other Exceptions
            {
                HataMesaji = string.Format("Exception. Message: {0} ", ex.Message);
            }
            finally
            {
                Conn.CloseConn();
            }

            if (HataMesaji != "")
            {
                MessageBox.Show(HataMesaji);
            }
        }

        private void pmImzalanlar_Opening(object sender, CancelEventArgs e)
        {
            miINGGonder.Enabled = Conn.INGEntVar;
        }

        private void btnIngKayitliKullaniciListesi_Click(object sender, EventArgs e)
        {
            string HataMesaji = "";
            try
            {
                btnIngKayitliKullaniciListesi.Enabled = false;
                Conn.OpenConn();
                INGsvc.ClientEInvoiceServicesPortClient cl = new INGsvc.ClientEInvoiceServicesPortClient();
                using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)cl.InnerChannel))
                {
                    string auth = string.Format("{0}:{1}", Conn.INGUserName, Conn.INGPassword);
                    byte[] bytarr = System.Text.Encoding.ASCII.GetBytes(auth);
                    string base64auth = Convert.ToBase64String(bytarr);
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add(HttpRequestHeader.Authorization, String.Format("Basic {0}", base64auth));
                    INGsvc.getUserListRequest ulr = new INGsvc.getUserListRequest();
                    ulr.Role = "PK";
                    ulr.VKN_TCKN = Conn.INGVKN;
                    ulr.Identifier = Conn.INGAlias;
                    INGsvc.UserType[] ut = cl.getUserList(ulr);
                    if (ut != null && ut.Count() > 0)
                    {
                        string delim = "";
                        string fmt = "select '{0}','{1}','{2}','{3}','{4}' \r\n";
                        string insertSql = "insert into genel.efaturakullanicilistesi(identifier, alias, title, idtype, registertime) \r\n";
                        Conn.ExecuteSql("delete from genel.efaturakullanicilistesi");
                        Conn.ExecuteSql("alter table genel.efaturakullanicilistesi auto_increment=0");
                        Conn.ExecuteSql("optimize table genel.efaturakullanicilistesi");
                        int k = 0;
                        foreach (INGsvc.UserType user in ut)
                        {
                            insertSql += delim + string.Format(fmt,
                                                     Conn.convert2Latin(user.Identifier.ToString()),
                                                     Conn.convert2Latin(user.Alias.ToString()),
                                                     Conn.convert2Latin(user.Title.ToString().Replace("'", "''")),
                                                     Conn.convert2Latin(user.Type.ToString()),
                                                     user.RegisterTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            delim = "union all \n\n";
                            k++;
                            if (k > 250)
                            {
                                Conn.ExecuteSql(insertSql);
                                insertSql = "insert into genel.efaturakullanicilistesi(identifier, alias, title, idtype, registertime) \r\n";
                                delim = "";
                                k = 0;
                            }

                        }
                        Conn.ExecuteSql(insertSql);
                    }
                }

            }
            catch (FaultException<INGsvc.ProcessingFault> fault) //Catches INGeF Processing Exceptions
            {
                HataMesaji = string.Format("ProcessingFault. Message: {0}    Reason: {1}   Error Code :{2}    Detail:{3}", fault.Message, fault.Reason, fault.Detail.Code, fault.Detail.Message);

                if (fault.InnerException != null)
                    HataMesaji += string.Format(" InnerExeption : {0}", fault.InnerException.Message);
            }
            catch (System.ServiceModel.FaultException fault) //Catches SOAP Exceptions
            {
                HataMesaji = string.Format("FaultException. Message: {0}    Reason: {1}   ", fault.Message, fault.Reason);

                if (fault.InnerException != null)
                    HataMesaji += string.Format(" InnerExeption : {0}", fault.InnerException.Message);
            }
            catch (HttpListenerException ex) //HttpListenerException, Unuthorized, Internet Server Error
            {
                HataMesaji = string.Format("HttpListenerException. Message: {0}  ErrorCode: {1}   NativeErrorCode:{2}   ", ex.Message, ex.ErrorCode, ex.NativeErrorCode);

                if (ex.InnerException != null)
                    HataMesaji += string.Format(" InnerExeption : {0}", ex.InnerException.Message);
            }
            catch (Exception ex) //Catches all other Exceptions
            {
                HataMesaji = string.Format("Exception. Message: {0} ", ex.Message);
            }
            finally
            {
                Conn.CloseConn();
                btnIngKayitliKullaniciListesi.Enabled = true;
            }

            if (HataMesaji != "")
            {
                MessageBox.Show(HataMesaji);
            }
        }

        private void btnZarfOlustur_Click(object sender, EventArgs e)
        {
            string saveXmlFolder = System.IO.Path.GetTempPath();
            string myInvoiceType = "", myInvoiceID = "", myInvoiceNumber = "", envUUID = "";
            try
            {
                Conn.OpenConn();

                foreach (DataGridViewRow row in dgView.SelectedRows)
                {
                    myInvoiceNumber = row.Cells["faturano"].Value.ToString();
                    myInvoiceID = row.Cells["id"].Value.ToString();
                    myInvoiceType = row.Cells["tip"].Value.ToString();
                    envUUID = CreateEnvolope(myInvoiceType, myInvoiceID, myInvoiceNumber);
                    if (envUUID != "")
                    {
                        File.Copy(saveXmlFolder + envUUID + ".xml", Conn.SaveXmlPath + myInvoiceNumber + '_' + envUUID + ".xml");
                        File.Delete(saveXmlFolder + envUUID + ".xml");
                        List<string> l = new List<string>();
                        l.Add(Conn.SaveXmlPath + myInvoiceNumber + '_' + envUUID + ".xml");
                        myCreateZipFile(Conn.SaveXmlPath + myInvoiceNumber + '_' + envUUID + ".zip", l);
                        l = null;
                    }
                }

            }
            finally
            {
                Conn.CloseConn();
            }
        }

        private void btnCariUPD_Click(object sender, EventArgs e)
        {
            try
            {
                btnCariUPD.Enabled = false;
                Conn.OpenConn();
                try
                {
                    string sql = "select id, faturavergino,cast(efaturakullan as char) as ekull, carino, caritipkodu, unvan, ekunvan, SIRKETUNVANI" +
                                 ", efaturafirstname, efaturalastname from carikarttanimlama where durum='Aktif' and ifnull(faturavergino,'')<>'' ";
                    string sGuncellenenler = "", sGuncellenmeyenler = "", sID = "", sTitle = "", sAlias = "", sCariTitle = "";
                    int iguncellenen = 0, iguncellenmeyen = 0, icnt = 0;
                    string saveFileName = System.IO.Path.GetTempPath() + "guncellenen" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    string saveFileNameGuncellenmeyen = System.IO.Path.GetTempPath() + "guncellenmeyen" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    using (var daCari = Conn.OpenAdapter(sql))
                    {
                        using (var dtCari = new DataTable())
                        {
                            daCari.Fill(dtCari);
                            foreach (DataRow cari in dtCari.Rows)
                            {
                                sID = Conn.getOnlyNumber(cari["faturavergino"].ToString());
                                sCariTitle = Conn.convert2UTF(cari["efaturafirstname"].ToString() + cari["efaturalastname"].ToString());
                                sql = "select {0} from genel.efaturakullanicilistesi where identifier='" + sID + "'";
                                icnt = Int32.Parse(Conn.ReadSingleField(string.Format(sql, "count(*)"), "0"));
                                sTitle = Conn.convert2UTF(Conn.ReadSingleField(string.Format(sql, "title"), "0"));
                                sAlias = Conn.convert2UTF(Conn.ReadSingleField(string.Format(sql, "alias"), "0"));
                                if (icnt > 0)
                                {
                                    try
                                    {
                                        if (cari["ekull"].ToString() == "1")
                                        {
                                            if (sCariTitle != sTitle)
                                            {
                                                sGuncellenenler = Conn.convert2UTF("'" +
                                                                   cari["carino"].ToString() + "'\t'" +
                                                                   cari["caritipkodu"].ToString() + "'\t'" +
                                                                   cari["faturavergino"].ToString() + "'\t'" +
                                                                   cari["unvan"].ToString() + "'\t'" +
                                                                   cari["ekunvan"].ToString() + "'\t'" +
                                                                   cari["SIRKETUNVANI"].ToString() + "'\r\n");
                                                if (File.Exists(saveFileName))
                                                {
                                                    File.AppendAllText(saveFileName, sGuncellenenler);
                                                }
                                                else
                                                {
                                                    File.WriteAllText(saveFileName, sGuncellenenler);
                                                }
                                                iguncellenen++;
                                                sql = "update carikarttanimlama set efaturafirstname='{0}', efaturalastname='{1}', " +
                                                    " efaturakullan='{2}', efaturaAlias='{3}', FATURAULKE='{4}' " +
                                                    " where id='{5}'";
                                                Conn.ExecuteSql(string.Format(sql,
                                                                              Conn.convert2Latin(Conn.Left(sTitle, 30)),
                                                                              Conn.convert2Latin(Conn.RightFromLeft(sTitle, 31)),
                                                                              "1",
                                                                              Conn.convert2Latin(sAlias),
                                                                              Conn.convert2Latin("Türkiye"),
                                                                              cari["id"].ToString()
                                                                              )
                                                                );
                                            }
                                            else
                                            {
                                                sGuncellenmeyenler =
                                                                   Conn.convert2UTF("'" +
                                                                   cari["carino"].ToString() + "'\t'" +
                                                                   cari["caritipkodu"].ToString() + "'\t'" +
                                                                   cari["faturavergino"].ToString() + "'\t'" +
                                                                   cari["unvan"].ToString() + "'\t'" +
                                                                   cari["ekunvan"].ToString() + "'\t'" +
                                                                   cari["SIRKETUNVANI"].ToString() + "'\r\n");
                                                if (File.Exists(saveFileNameGuncellenmeyen))
                                                {
                                                    File.AppendAllText(saveFileNameGuncellenmeyen, sGuncellenmeyenler);
                                                }
                                                else
                                                {
                                                    File.WriteAllText(saveFileNameGuncellenmeyen, sGuncellenmeyenler);
                                                }
                                                iguncellenmeyen++;
                                            }
                                        }
                                        else
                                        {
                                            sGuncellenenler = Conn.convert2UTF("'" +
                                                               cari["carino"].ToString() + "'\t'" +
                                                               cari["caritipkodu"].ToString() + "'\t'" +
                                                               cari["faturavergino"].ToString() + "'\t'" +
                                                               cari["unvan"].ToString() + "'\t'" +
                                                               cari["ekunvan"].ToString() + "'\t'" +
                                                               cari["SIRKETUNVANI"].ToString() + "'\r\n");
                                            if (File.Exists(saveFileName))
                                            {
                                                File.AppendAllText(saveFileName, sGuncellenenler);
                                            }
                                            else
                                            {
                                                File.WriteAllText(saveFileName, sGuncellenenler);
                                            }
                                            iguncellenen++;
                                            sql = "update carikarttanimlama set efaturafirstname='{0}', efaturalastname='{1}', " +
                                                    " efaturakullan='{2}', efaturaAlias='{3}', FATURAULKE='{4}' " +
                                                    " where id='{5}'";
                                            Conn.ExecuteSql(string.Format(sql,
                                                                          Conn.convert2Latin(Conn.Left(sTitle, 30)),
                                                                          Conn.convert2Latin(Conn.RightFromLeft(sTitle, 31)),
                                                                          "1",
                                                                          Conn.convert2Latin(sAlias),
                                                                          Conn.convert2Latin("Türkiye"),
                                                                          cari["id"].ToString()
                                                                          )
                                                            );

                                        }
                                    }
                                    catch (Exception exp)
                                    {
                                        MessageBox.Show("Cari Güncellerken Hata Oluştu\r\n" + cari["id"].ToString() + " " + cari["unvan"].ToString() + "\r\n" + exp.Message);
                                    }
                                }
                            }
                            dtCari.Clear();
                            GC.SuppressFinalize(dtCari);
                            GC.Collect();
                        }
                        GC.SuppressFinalize(daCari);
                        GC.Collect();
                    }


                    if (iguncellenen > 0 || iguncellenmeyen > 0)
                    {
                        if (File.Exists(saveFileName)) sGuncellenenler = File.ReadAllText(saveFileName);
                        if (File.Exists(saveFileNameGuncellenmeyen)) sGuncellenmeyenler = File.ReadAllText(saveFileNameGuncellenmeyen);

                        string stmp = "Güncellenen cariler\r\nCariNo\tCTip\tVergiNo\tÜnvan\tEkÜnvan\tŞirketÜnvan\r\n" + sGuncellenenler + "\r\n\r\n" +
                                    "Güncellenemeyenn cariler\r\nCariNo\r\nCTip\r\nVergiNo\r\nÜnvan\r\nEkÜnvan\r\nŞirketÜnvan\r\n" + sGuncellenmeyenler;
                        saveFileName = System.IO.Path.GetTempPath() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

                        File.WriteAllText(saveFileName, stmp);

                        Clipboard.SetText(stmp);

                        DialogResult dr = MessageBox.Show("Güncellemeler yapıldı.\r\nDeğişiklikler panoya kopyalandı.\r\n" +
                                                        "Metin editörü veya excel yapıştırarak inceleyebilirsiniz\r\n" +
                                                        "Ayrıca log olarak '" + saveFileName + "' dosyasına kaydedildi\r\n" +
                                                        "Dosyayı açmak ister misiniz?", "INGef Guncelleme Yapıldı", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string args = "notepad \"" + saveFileName + "\"";
                            Conn.RunShell(args, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cari Kartlar Güncellenirken Hata Oluştu.\r\n", ex.Message);
                }

                GC.Collect();

                try
                {
                    string sql = "select id, faturavergino,cast(efaturakullan as char) as ekull, carino, caritipkodu, unvan, ekunvan, SIRKETUNVANI" +
                                 ", efaturafirstname, efaturalastname from carikarttanimlama where durum='Aktif' and ifnull(faturavergino,'')<>'' " +
                                 " and efaturakullan=1 ";
                    string sGuncellenenler = "", sID = "";
                    using (var daCari = Conn.OpenAdapter(sql))
                    {
                        using (var dtCari = new DataTable())
                        {
                            daCari.Fill(dtCari);
                            foreach (DataRow cari in dtCari.Rows)
                            {
                                sID = Conn.getOnlyNumber(cari["faturavergino"].ToString());
                                sql = "select * from genel.efaturakullanicilistesi where identifier='" + sID + "'";
                                string stitle = Conn.ReadSingleField(sql, "title");
                                if (stitle == "")
                                {
                                    try
                                    {
                                        sGuncellenenler += sGuncellenenler +
                                                                       Conn.convert2UTF("'" +
                                                                       cari["carino"].ToString() + "'\t'" +
                                                                       cari["caritipkodu"].ToString() + "'\t'" +
                                                                       cari["faturavergino"].ToString() + "'\t'" +
                                                                       cari["unvan"].ToString() + "'\t'" +
                                                                       cari["ekunvan"].ToString() + "'\t'" +
                                                                       cari["SIRKETUNVANI"].ToString() + "'\r\n");
                                        sql = "update carikarttanimlama set efaturafirstname='', efaturalastname='', " +
                                              " efaturakullan=0 where id='{0}'";
                                        Conn.ExecuteSql(string.Format(sql, cari["id"].ToString()));
                                    }
                                    catch (Exception exp)
                                    {
                                        MessageBox.Show("İptal Cariler Güncellenirken Hata Oluştu\r\n" + cari["id"].ToString() + " " + cari["unvan"].ToString() + "\r\n" + exp.Message);
                                    }
                                }
                            }
                            dtCari.Clear();
                            GC.SuppressFinalize(dtCari);
                        }
                        GC.SuppressFinalize(daCari);
                    }
                    GC.Collect();

                    if (sGuncellenenler != "")
                    {
                        string stmp = "Güncellenen cariler\r\nCariNo\tCTip\tVergiNo\tÜnvan\tEkÜnvan\tŞirketÜnvan\r\n" + sGuncellenenler;
                        string saveFileName = System.IO.Path.GetTempPath() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

                        File.WriteAllText(saveFileName, stmp);

                        Clipboard.SetText(stmp);

                        DialogResult dr = MessageBox.Show("Güncellemeler yapıldı.\r\nDeğişiklikler panoya kopyalandı.\r\n" +
                                                        "Metin editörü veya excel yapıştırarak inceleyebilirsiniz\r\n" +
                                                        "Ayrıca log olarak '" + saveFileName + "' dosyasına kaydedildi\r\n" +
                                                        "Dosyayı açmak ister misiniz?", "INGef Guncelleme Yapıldı", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string args = "notepad \"" + saveFileName + "\"";
                            Conn.RunShell(args, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cari Kartlar Güncellenirken Hata Oluştu.\r\n", ex.Message);
                }
                GC.Collect();
            }
            finally
            {
                btnCariUPD.Enabled = true;
                Conn.CloseConn();
            }
        }
    }
}