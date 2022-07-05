


svcutil /t:code /ser:XmlSerializer http://beplinktest.vba.va.gov/BenefitClaimServiceBean/BenefitClaimWebServce?wsdl

svcutil /t:metadata http://beplinktest.vba.va.gov/BenefitClaimServiceBean/BenefitClaimWebService?wsdl 


del BenefitClaimWebService.wsdl
del BenefitClaimWebService.xsd


rename services.share.benefits.vba.va.gov.wsdl BenefitClaimWebService.wsdl
rename services.share.benefits.vba.va.gov.xsd BenefitClaimWebService.xsd
