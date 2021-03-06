


/****** Object:  Table [dbo].[Form0820Payments]    Script Date: 7/17/2019 7:19:00 PM ******/
DROP TABLE IF EXISTS [dbo].[Form0820Payments]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Form0820Payments](
	[udo_form0820paymentsid] [nvarchar](36) NOT NULL,
	[transactioncurrencyid] [nvarchar](36) NULL,
	[transactioncurrencyidname] [nvarchar](100) NULL,
	[AdditionalInformation] [nvarchar](250) NULL,
	[Amount] [money] NULL,
	[CheckEndorsedPmtValue] [bit] NULL,
	[CheckEndorsedPmt] [nvarchar](100) NULL,
	[CheckNotEndorsedPmtValue] [bit] NULL,
	[CheckNotEndorsedPmt] [nvarchar](100) NULL,
	[CPBValue] [bit] NULL,
	[CPB] [nvarchar](100) NULL,
	[PmtDateValue] [datetime] NULL,
	[PmtDate] [nvarchar](50) NULL,
	[DirDepValue] [bit] NULL,
	[DirDep] [nvarchar](100) NULL,
	[EDUValue] [bit] NULL,
	[EDU] [nvarchar](100) NULL,
	[PotentialFraudValue] [bit] NULL,
	[PotentialFraud] [nvarchar](100) NULL,
	[VetsNetValue] [bit] NULL,
	[VetsNet] [nvarchar](100) NULL,
	[VREValue] [bit] NULL,
	[VRE] [nvarchar](100) NULL,
	[BenefitTypeValue] [int] NULL,
	[BenefitType] [nvarchar](500) NULL,
	[PaymentSystemValue] [int] NULL,
	[PaymentSystem] [nvarchar](500) NULL,
	[PaymentTypeValue] [int] NULL,
	[PaymentType] [nvarchar](500) NULL,
	[udo_servicerequestid] [nvarchar](36) NULL,
	[udo_servicerequestidName] [nvarchar](500) NULL,
 CONSTRAINT [PK_Form0820Payments] PRIMARY KEY CLUSTERED 
(
	[udo_form0820paymentsid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LetterGeneration]    Script Date: 7/17/2019 7:19:00 PM ******/
DROP TABLE IF EXISTS [dbo].[LetterGeneration]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LetterGeneration](
	[udo_lettergenerationid] [nvarchar](36) NOT NULL,
	[transactioncurrencyid] [nvarchar](36) NULL,
	[transactioncurrencyidname] [nvarchar](100) NULL,
	[udo_reqnumber] [nvarchar](100) NULL,
	[udo_filenumber] [nvarchar](100) NULL,
	[DODValue] [datetime] NULL,
	[DOD] [nvarchar](50) NULL,
	[udo_injurydiseasedisableddateValue] [datetime] NULL,
	[udo_injurydiseasedisableddate] [nvarchar](50) NULL,
	[udo_blindnesseffectivedateValue] [datetime] NULL,
	[udo_blindnesseffectivedate] [nvarchar](50) NULL,
	[udo_limblosseffectivedateValue] [datetime] NULL,
	[udo_limblosseffectivedate] [nvarchar](50) NULL,
	[EvaluationValue] [bit] NULL,
	[Evaluation] [nvarchar](100) NULL,
	[VetSSN] [nvarchar](100) NULL,
	[ClaimNumber] [nvarchar](100) NULL,
	[udo_mailing_address1] [nvarchar](200) NULL,
	[udo_mailing_address2] [nvarchar](200) NULL,
	[udo_mailing_address3] [nvarchar](100) NULL,
	[udo_mailing_city] [nvarchar](100) NULL,
	[udo_mailing_state] [nvarchar](100) NULL,
	[udo_mailing_zip] [nvarchar](100) NULL,
	[udo_mailingcountry] [nvarchar](100) NULL,
	[VetBranch] [nvarchar](100) NULL,
	[SRdescription] [varchar](max) NULL,
	[SRbranch] [varchar](max) NULL,
	[SRraddate] [varchar](max) NULL,
	[SReoddate] [varchar](max) NULL,
	[SRratingdegree] [nvarchar](100) NULL,
	[SRratingeffectivedate] [nvarchar](100) NULL,
	[NetAmount] [money] NULL,
	[AAAmount] [money] NULL,
	[PensionBenefit] [money] NULL,
	[EffectiveDateValue] [datetime] NULL,
	[EffectiveDate] [nvarchar](50) NULL,
	[FutureExamDateValue] [datetime] NULL,
	[FutureExamDate] [nvarchar](50) NULL,
	[udo_letteraddressingValue] [int] NULL,
	[udo_letteraddressing] [nvarchar](500) NULL,
	[Enclosures] [varchar](max) NULL,
	[udo_srfirstname] [nvarchar](100) NULL,
	[udo_srlastname] [nvarchar](100) NULL,
	[CurrentMonthlyRate] [money] NULL,
	[PrefixValue] [int] NULL,
	[Prefix] [nvarchar](500) NULL,
	[udo_address1] [nvarchar](200) NULL,
	[udo_address2] [nvarchar](200) NULL,
	[udo_address3] [nvarchar](100) NULL,
	[udo_city] [nvarchar](30) NULL,
	[udo_state] [nvarchar](100) NULL,
	[udo_zipcode] [nvarchar](100) NULL,
	[udo_country] [nvarchar](100) NULL,
	[DischargeTypes] [varchar](max) NULL,
	[Disabilities] [varchar](max) NULL,
	[DisabilityPercent] [varchar](max) NULL,
	[LostLimbOrBlindValue] [bit] NULL,
	[LostLimbOrBlind] [nvarchar](100) NULL,
	[Discharge] [nvarchar](100) NULL,
	[ServiceDates] [nvarchar](100) NULL,
	[DiedInActiveDutyValue] [bit] NULL,
	[DiedInActiveDuty] [nvarchar](100) NULL,
	[DiedToDisabilityValue] [bit] NULL,
	[DiedToDisability] [nvarchar](100) NULL,
	[DisabilityIndValue] [bit] NULL,
	[DisabilityInd] [nvarchar](100) NULL,
	[ReceivedGrantValue] [bit] NULL,
	[ReceivedGrant] [nvarchar](100) NULL,
	[EntitledToHigherDisabilityValue] [bit] NULL,
	[EntitledToHigherDisability] [nvarchar](100) NULL,
	[BenefitType] [nvarchar](100) NULL,
	[AwardBenefitType] [nvarchar](100) NULL,
	[FaxDescription] [varchar](max) NULL,
	[PaymentAmount] [money] NULL,
	[FaxPages] [nvarchar](100) NULL,
	[PayDateValue] [datetime] NULL,
	[PayDate] [nvarchar](50) NULL,
	[FaxNum] [nvarchar](100) NULL,
	[OwnerManager] [nvarchar](100) NULL,
	[ReferReply] [nvarchar](11) NULL,
	[OwnerFileNum] [nvarchar](100) NULL,
	[CCaddress1] [nvarchar](250) NULL,
	[CCstate] [nvarchar](50) NULL,
	[CCzip] [nvarchar](20) NULL,
	[CCcity] [nvarchar](80) NULL,
	[CCaddress2] [nvarchar](250) NULL,
	[ManagerLast] [nvarchar](64) NULL,
	[ManagerTitle] [nvarchar](128) NULL,
	[ManagerFirst] [nvarchar](64) NULL,
	[ic_va_address2] [nvarchar](100) NULL,
	[ic_va_address3] [nvarchar](100) NULL,
	[ic_va_address1] [nvarchar](100) NULL,
	[ic_va_state] [nvarchar](100) NULL,
	[ic_va_city] [nvarchar](100) NULL,
	[ic_va_zipcode] [nvarchar](100) NULL,
	[ic_va_localfax] [nvarchar](100) NULL,
	[ic_va_alias] [nvarchar](200) NULL,
	[ic_va_faxnumber] [nvarchar](100) NULL,
	[ic_udo_returnmailingaddress] [varchar](max) NULL,
	[pc_va_address2] [nvarchar](100) NULL,
	[pc_va_address3] [nvarchar](100) NULL,
	[pc_va_address1] [nvarchar](100) NULL,
	[pc_va_state] [nvarchar](100) NULL,
	[pc_va_city] [nvarchar](100) NULL,
	[pc_va_zipcode] [nvarchar](100) NULL,
	[pc_va_localfax] [nvarchar](100) NULL,
	[pc_va_alias] [nvarchar](200) NULL,
	[pc_va_faxnumber] [nvarchar](100) NULL,
	[pc_udo_returnmailingaddress] [varchar](max) NULL,
	[vet_lastname] [nvarchar](50) NULL,
	[vet_middlename] [nvarchar](50) NULL,
	[vet_firstname] [nvarchar](50) NULL,
	[DependentAmount] [money] NULL,
	[udo_depfirstname] [nvarchar](100) NULL,
	[udo_deplastname] [nvarchar](100) NULL,
	[udo_dateofbirth] [nvarchar](500) NULL,
	[udo_dateofbirthValue] [datetime] NULL,
	[udo_firstname] [nvarchar](100) NULL,
	[udo_lastname] [nvarchar](100) NULL,
	[va_replyreferto] [nvarchar](11) NULL,
	[CCaddress3] [nvarchar](250) NULL,
	[udo_evaluationconsideredpermanent] [nvarchar](500) NULL,
	[udo_evaluationconsideredpermanentValue] [bit] NULL,
	[NetAmountValue] [money] NULL,
	[AAAmountValue] [money] NULL,
	[PensionBenefitValue] [money] NULL,
	[CurrentMonthlyRateValue] [money] NULL,
	[PaymentAmountValue] [money] NULL,
	[DependentAmountValue] [money] NULL,
	[DiagnosticCodes] [varchar](max) NULL,
	[udo_moderror] [nvarchar](500) NULL,
	[udo_moderrorValue] [int] NULL,
	[soj_va_name] [nvarchar](100) NULL,
	[MPList] [varchar](max) NULL,
 CONSTRAINT [PK_LetterGeneration] PRIMARY KEY CLUSTERED 
(
	[udo_lettergenerationid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LetterGenerationDisability]    Script Date: 7/17/2019 7:19:00 PM ******/
DROP TABLE IF EXISTS [dbo].[LetterGenerationDisability]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LetterGenerationDisability](
	[udo_lettergenerationdisabilityid] [nvarchar](36) NOT NULL,
	[transactioncurrencyid] [nvarchar](36) NULL,
	[transactioncurrencyidname] [nvarchar](100) NULL,
	[udo_percentage] [nvarchar](100) NULL,
	[udo_lettergenerationid] [nvarchar](36) NULL,
	[udo_lettergenerationidName] [nvarchar](500) NULL,
	[udo_effectivedate] [nvarchar](100) NULL,
	[udo_disability] [nvarchar](1000) NULL,
	[udo_diagnosticcode] [nvarchar](100) NULL,
 CONSTRAINT [PK_LetterGenerationDisability] PRIMARY KEY CLUSTERED 
(
	[udo_lettergenerationdisabilityid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceRequest]    Script Date: 7/17/2019 7:19:00 PM ******/
DROP TABLE IF EXISTS [dbo].[ServiceRequest]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceRequest](
	[udo_servicerequestid] [nvarchar](36) NOT NULL,
	[transactioncurrencyid] [nvarchar](36) NULL,
	[transactioncurrencyidname] [nvarchar](100) NULL,
	[DODValue] [datetime] NULL,
	[DOD] [nvarchar](50) NULL,
	[udo_typeofcontactValue] [int] NULL,
	[udo_typeofcontact] [nvarchar](500) NULL,
	[VetEmail] [nvarchar](100) NULL,
	[udo_reqnumber] [nvarchar](100) NULL,
	[POA] [varchar](max) NULL,
	[VetAddress1] [nvarchar](200) NULL,
	[VetAddress2] [nvarchar](200) NULL,
	[VetAddress3] [nvarchar](100) NULL,
	[VetCity] [nvarchar](100) NULL,
	[VetState] [nvarchar](100) NULL,
	[VetZip] [nvarchar](100) NULL,
	[udo_mailingcountry] [nvarchar](100) NULL,
	[udo_filenumber] [nvarchar](100) NULL,
	[EveningPhone] [nvarchar](100) NULL,
	[DayPhone] [nvarchar](200) NULL,
	[CallerPhone] [nvarchar](200) NULL,
	[udo_description] [varchar](max) NULL,
	[CallerFirstName] [nvarchar](100) NULL,
	[CallerLastName] [nvarchar](100) NULL,
	[CallerStreet1] [nvarchar](200) NULL,
	[CallerStreet2] [nvarchar](200) NULL,
	[CallerStreet3] [nvarchar](100) NULL,
	[CallerCity] [nvarchar](30) NULL,
	[CallerState] [nvarchar](100) NULL,
	[CallerZip] [nvarchar](100) NULL,
	[udo_country] [nvarchar](100) NULL,
	[udo_readscriptValue] [bit] NULL,
	[udo_readscript] [nvarchar](100) NULL,
	[udo_relationtoveteranValue] [int] NULL,
	[udo_relationtoveteran] [nvarchar](500) NULL,
	[udo_enroutetovaValue] [bit] NULL,
	[udo_enroutetova] [nvarchar](100) NULL,
	[DependentFirstName] [nvarchar](100) NULL,
	[DependentLastName] [nvarchar](100) NULL,
	[DependentAddress] [nvarchar](100) NULL,
	[DependentState] [nvarchar](100) NULL,
	[DependentCity] [nvarchar](100) NULL,
	[DependentZip] [nvarchar](100) NULL,
	[DependentDOBValue] [datetime] NULL,
	[DependentDOB] [nvarchar](50) NULL,
	[DependentSSN] [nvarchar](100) NULL,
	[DependentAddresses] [varchar](max) NULL,
	[PlaceOfDeath] [nvarchar](300) NULL,
	[DependentNames] [varchar](max) NULL,
	[udo_21534Value] [bit] NULL,
	[udo_21534] [nvarchar](100) NULL,
	[DeathChecklistValue] [bit] NULL,
	[DeathChecklist] [nvarchar](100) NULL,
	[udo_21530Value] [bit] NULL,
	[udo_21530] [nvarchar](100) NULL,
	[NOKLetterValue] [bit] NULL,
	[NOKLetter] [nvarchar](100) NULL,
	[ProcessedFnodExp] [nvarchar](4000) NULL,
	[OtherValue] [bit] NULL,
	[Other] [nvarchar](100) NULL,
	[OtherSpec] [nvarchar](100) NULL,
	[VetLookedUpValue] [bit] NULL,
	[VetLookedUp] [nvarchar](100) NULL,
	[udo_401330Value] [bit] NULL,
	[udo_401330] [nvarchar](100) NULL,
	[udo_benefitsstoppedValue] [bit] NULL,
	[udo_benefitsstopped] [nvarchar](100) NULL,
	[PMCValue] [bit] NULL,
	[PMC] [nvarchar](100) NULL,
	[ProcessedFnodValue] [bit] NULL,
	[ProcessedFnod] [nvarchar](100) NULL,
	[udo_possibleburialinnationalcemeteryValue] [bit] NULL,
	[udo_possibleburialinnationalcemetery] [nvarchar](100) NULL,
	[udo_benefitsstopfirstofmonthValue] [bit] NULL,
	[udo_benefitsstopfirstofmonth] [nvarchar](100) NULL,
	[udo_willroutereportofdeathValue] [bit] NULL,
	[udo_willroutereportofdeath] [nvarchar](100) NULL,
	[udo_dateofmissingpaymentValue] [datetime] NULL,
	[udo_dateofmissingpayment] [nvarchar](50) NULL,
	[udo_amtofpayments] [decimal](18, 2) NULL,
	[udo_paymentissuedviaValue] [int] NULL,
	[udo_paymentissuedvia] [nvarchar](500) NULL,
	[udo_checkendorsedandlostValue] [int] NULL,
	[udo_checkendorsedandlost] [nvarchar](500) NULL,
	[udo_typeofpaymentValue] [int] NULL,
	[udo_typeofpayment] [nvarchar](500) NULL,
	[udo_checkendorsedandstolenValue] [int] NULL,
	[udo_checkendorsedandstolen] [nvarchar](500) NULL,
	[udo_paymentmethodValue] [int] NULL,
	[udo_paymentmethod] [nvarchar](500) NULL,
	[udo_addresschangedValue] [bit] NULL,
	[udo_addresschanged] [nvarchar](100) NULL,
	[DeceasedNVBValue] [bit] NULL,
	[DeceasedNVB] [nvarchar](100) NULL,
	[NameNVB] [nvarchar](100) NULL,
	[PayeeCode] [nvarchar](20) NULL,
	[BenSSN] [nvarchar](20) NULL,
	[Payment] [nvarchar](200) NULL,
	[BenName] [nvarchar](100) NULL,
	[udo_fnodreportingforValue] [int] NULL,
	[udo_fnodreportingfor] [nvarchar](500) NULL,
	[UpdateAddrValue] [bit] NULL,
	[UpdateAddr] [nvarchar](100) NULL,
	[UserOffice] [nvarchar](100) NULL,
	[StationNumber] [nvarchar](100) NULL,
	[ReplyRefer] [nvarchar](11) NULL,
	[UserLastName] [nvarchar](64) NULL,
	[UserTitle] [nvarchar](128) NULL,
	[UserFirstName] [nvarchar](64) NULL,
	[vet_lastname] [nvarchar](50) NULL,
	[vet_middlename] [nvarchar](50) NULL,
	[vet_firstname] [nvarchar](50) NULL,
	[ActionToBeCompleted] [varchar](max) NULL,
	[udo_servicerequestsidudo_servicerequestsidName] [nvarchar](500) NULL,
	[udo_interactionid] [nvarchar](36) NULL,
	[udo_servicerequestsid] [nvarchar](36) NULL,
	[udo_servicerequestsidName] [nvarchar](500) NULL,
	[udo_haspoa] [nvarchar](500) NULL,
	[udo_haspoaValue] [bit] NULL,
	[udo_srfirstname] [nvarchar](100) NULL,
	[udo_srlastname] [nvarchar](100) NULL,
	[udo_requesttype] [nvarchar](100) NULL,
	[udo_requestsubtype] [nvarchar](100) NULL,
	[udo_placeofdeath] [nvarchar](300) NULL,
	[DOBDeceased] [nvarchar](500) NULL,
	[DOBDeceasedValue] [datetime] NULL,
	[udo_dependentnames] [varchar](max) NULL,
	[udo_lookedupvetrecord] [nvarchar](500) NULL,
	[udo_lookedupvetrecordValue] [bit] NULL,
	[udo_nokletter] [nvarchar](500) NULL,
	[udo_nokletterValue] [bit] NULL,
	[udo_other] [nvarchar](500) NULL,
	[udo_otherValue] [bit] NULL,
 CONSTRAINT [PK_ServiceRequest] PRIMARY KEY CLUSTERED 
(
	[udo_servicerequestid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [CheckEndorsedPmtValue]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [CheckNotEndorsedPmtValue]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [CPBValue]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [DirDepValue]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [EDUValue]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [PotentialFraudValue]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [VetsNetValue]
GO
ALTER TABLE [dbo].[Form0820Payments] ADD  DEFAULT ((0)) FOR [VREValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [EvaluationValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [NetAmount]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [AAAmount]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [PensionBenefit]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [CurrentMonthlyRate]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [LostLimbOrBlindValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [DiedInActiveDutyValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [DiedToDisabilityValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [DisabilityIndValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [ReceivedGrantValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [EntitledToHigherDisabilityValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [PaymentAmount]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [DependentAmount]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [udo_evaluationconsideredpermanentValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [NetAmountValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [AAAmountValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [PensionBenefitValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [CurrentMonthlyRateValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [PaymentAmountValue]
GO
ALTER TABLE [dbo].[LetterGeneration] ADD  DEFAULT ((0)) FOR [DependentAmountValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_readscriptValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_enroutetovaValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_21534Value]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [DeathChecklistValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_21530Value]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [NOKLetterValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [OtherValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [VetLookedUpValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_401330Value]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_benefitsstoppedValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [PMCValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [ProcessedFnodValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_possibleburialinnationalcemeteryValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_benefitsstopfirstofmonthValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_willroutereportofdeathValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_amtofpayments]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_addresschangedValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [DeceasedNVBValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [UpdateAddrValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_haspoaValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_lookedupvetrecordValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_nokletterValue]
GO
ALTER TABLE [dbo].[ServiceRequest] ADD  DEFAULT ((0)) FOR [udo_otherValue]
GO
