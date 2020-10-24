using System;
using System.Diagnostics;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.certificate.validation;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.check.certificate;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
//using xmlsig.samples.utils;

namespace ubltr
{
    /**
     * Provides validation functions for certificates
     * @author: suleyman.uslu
     */
    public class CertValidation 
    {
        /**
         * Validates given certificate
         */
        public static Boolean validateCertificate(ECertificate certificate)
        {
            try
            {

                ValidationSystem vs;
                // generate policy which going to be used in validation
                if (certificate.isMaliMuhurCertificate())
                {
                    ValidationPolicy policy = new ValidationPolicy();
                    String policyPath = Conn.ROOT_DIR + "efatura\\config\\certval-policy-malimuhur.xml";
                    policy = PolicyReader.readValidationPolicy(policyPath);
                    vs = CertificateValidation.createValidationSystem(policy);
                }
                else
                {
                    ValidationPolicy policy = new ValidationPolicy();
                    String policyPath = Conn.ROOT_DIR + "efatura\\config\\certval-policy.xml";
                    policy = PolicyReader.readValidationPolicy(policyPath);
                    vs = CertificateValidation.createValidationSystem(policy);
                }
                vs.setBaseValidationTime(DateTime.UtcNow);

                // validate certificate
                CertificateStatusInfo csi = CertificateValidation.validateCertificate(vs, certificate);

                // return true if certificate is valid, false otherwise
                if (csi.getCertificateStatus() != CertificateStatus.VALID)
                    return false;
                else if (csi.getCertificateStatus() == CertificateStatus.VALID)
                    return true;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while validating certificate", e);
            }
            return false;
        }

    }
}
