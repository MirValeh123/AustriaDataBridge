using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Application.External.Taxograf.Models;
using Application.External.Taxograf.Models.Dtos;
using Application.External.Taxograf.Models.Base;

namespace Application.Converters
{
    /// <summary>
    /// ManufactureApiResponse-ni controlCardRequest XML formatına çevirən converter implementation
    /// </summary>
    public class ControlCardRequestXmlConverter : IControlCardRequestXmlConverter
    {
        public string ConvertToXml(ManufactureApiResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var totalCards = CalculateTotalCards(response);
            var jobInformation = ResolveJobInformation(response, totalCards);
            var xmlDocument = BuildXmlDocument(response, jobInformation);

            return FormatXmlDocument(xmlDocument);
        }

        private int CalculateTotalCards(ManufactureApiResponse response)
        {
            var driverCards = response.DriverCardsExportModel?.CardAmount > 0
                ? response.DriverCardsExportModel.CardAmount
                : response.DriverCardsExportModel?.ExportModels?.Count ?? 0;

            var transporterCards = response.TransporterCardsExportModel?.CardAmount > 0
                ? response.TransporterCardsExportModel.CardAmount
                : response.TransporterCardsExportModel?.ExportModels?.Count ?? 0;

            var workshopCards = response.WorkshopCardsExportModel?.CardAmount > 0
                ? response.WorkshopCardsExportModel.CardAmount
                : response.WorkshopCardsExportModel?.ExportModels?.Count ?? 0;

            var inspectorCards = response.InspectorCardsExportModel?.CardAmount > 0
                ? response.InspectorCardsExportModel.CardAmount
                : response.InspectorCardsExportModel?.ExportModels?.Count ?? 0;

            return driverCards + transporterCards + workshopCards + inspectorCards;
        }

        private XDocument BuildXmlDocument(ManufactureApiResponse response, CardExportWrapper jobInformation)
        {
            if (jobInformation == null)
                throw new InvalidOperationException("Job information could not be resolved.");

            var root = new XElement("controlCardRequest",
                new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),

                new XElement("data",
                    BuildJobInformationElement(jobInformation),
                    BuildCardRequestElement(response)
                )
            );

            return new XDocument(new XDeclaration("1.0", "utf-8", null), root);
        }

        private XElement BuildJobInformationElement(CardExportWrapper jobInformation)
        {
            return new XElement("jobInformation",
                new XElement("jobNumber", jobInformation.JobNumber ),
                new XElement("creationTime", jobInformation.CreationTime.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("equipmentType", jobInformation.EquipmentType),
                new XElement("cardAmount", jobInformation.CardAmount),
                new XElement("cardIssuingMemberState", jobInformation.CardIssuingMemberState ?? string.Empty)
            );
        }

        private XElement BuildCardRequestElement(ManufactureApiResponse response)
        {
            var cardElements = new List<XElement>();

            cardElements.AddRange(response.DriverCardsExportModel?.ExportModels?.Select(CreateDriverCardElement) ?? Enumerable.Empty<XElement>());
            cardElements.AddRange(response.TransporterCardsExportModel?.ExportModels?.Select(CreateTransporterCardElement) ?? Enumerable.Empty<XElement>());
            cardElements.AddRange(response.WorkshopCardsExportModel?.ExportModels?.Select(CreateWorkshopCardElement) ?? Enumerable.Empty<XElement>());
            cardElements.AddRange(response.InspectorCardsExportModel?.ExportModels?.Select(CreateInspectorCardElement) ?? Enumerable.Empty<XElement>());

            return new XElement("cardRequest", cardElements);
        }

        private string FormatXmlDocument(XDocument document)
        {
            using (var writer = new System.IO.StringWriter())
            {
                using (var xmlWriter = System.Xml.XmlWriter.Create(writer, new System.Xml.XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false
                }))
                {
                    document.WriteTo(xmlWriter);
                }
                return writer.ToString();
            }
        }

        private XElement CreateDriverCardElement(DriverCardExportDto dto)
        {
            var cardElements = new List<XElement>
            {
                new XElement("Test", dto.Test),
                new XElement("cardNumber", dto.CardNumber ?? string.Empty),
                new XElement("cardIssueDate", FormatDateTime(dto.CardIssueDate)),
                new XElement("cardValidityBegin", FormatDateTime(dto.CardValidityBegin)),
                new XElement("cardExpiryDate", FormatDateTime(dto.CardExpiryDate))
            };

            AddCardHolderIfExists(cardElements, dto.CardHolder);
            AddCardIssuingAuthorityIfExists(cardElements, dto.CardIssuingAuthority);
            AddDrivingLicenceInfoIfExists(cardElements, dto.DrivingLicenceInfo);
            AddBinaryDataIfExists(cardElements, dto.BinaryData);

            return new XElement("card", cardElements);
        }

        private XElement CreateTransporterCardElement(TransporterCardExportDto dto)
        {
            var cardElements = new List<XElement>
            {
                new XElement("Test", dto.Test),
                new XElement("cardNumber", dto.CardNumber ?? string.Empty),
                new XElement("cardIssueDate", FormatDateTime(dto.CardIssueDate)),
                new XElement("cardValidityBegin", FormatDateTime(dto.CardValidityBegin)),
                new XElement("cardExpiryDate", FormatDateTime(dto.CardExpiryDate))
            };

            AddCardHolderIfExists(cardElements, dto.CardHolder);
            AddCardIssuingAuthorityIfExists(cardElements, dto.CardIssuingAuthority);
            AddCompanyIfExists(cardElements, dto.Company);

            return new XElement("card", cardElements);
        }

        private XElement CreateWorkshopCardElement(WorkShopCardExportDto dto)
        {
            var cardElements = new List<XElement>
            {
                new XElement("Test", dto.Test),
                new XElement("cardNumber", dto.CardNumber ?? string.Empty),
                new XElement("cardIssueDate", FormatDateTime(dto.CardIssueDate)),
                new XElement("cardValidityBegin", FormatDateTime(dto.CardValidityBegin)),
                new XElement("cardExpiryDate", FormatDateTime(dto.CardExpiryDate))
            };

            AddCardHolderIfExists(cardElements, dto.CardHolder);
            AddCardIssuingAuthorityIfExists(cardElements, dto.CardIssuingAuthority);
            AddWorkshopIfExists(cardElements, dto.Workshop);
            AddBinaryDataIfExists(cardElements, dto.BinaryData);

            return new XElement("card", cardElements);
        }

        private XElement CreateInspectorCardElement(InspectorCardExportDto dto)
        {
            var cardElements = new List<XElement>
            {
                new XElement("Test", dto.Test),
                new XElement("cardNumber", dto.CardNumber ?? string.Empty),
                new XElement("cardIssueDate", FormatDateTime(dto.CardIssueDate)),
                new XElement("cardValidityBegin", FormatDateTime(dto.CardValidityBegin)),
                new XElement("cardExpiryDate", FormatDateTime(dto.CardExpiryDate))
            };

            AddCardHolderIfExists(cardElements, dto.CardHolder);
            AddCardIssuingAuthorityIfExists(cardElements, dto.CardIssuingAuthority);
            AddControlBodyIfExists(cardElements, dto.ControlBody);

            return new XElement("card", cardElements);
        }

        private void AddCardHolderIfExists(List<XElement> cardElements, ExportModelCardHolderBase holder)
        {
            if (holder == null) return;

            var cardHolderElement = new XElement("cardHolder",
                new XElement("cardHolderSurnameAzari", holder.CardHolderSurnameAzari ?? string.Empty),
                new XElement("cardHolderSurnameLatin", holder.CardHolderSurnameLatin ?? string.Empty),
                new XElement("cardHolderFirstNamesAzari", holder.CardHolderFirstNamesAzari ?? string.Empty),
                new XElement("cardHolderFirstNamesLatin", holder.CardHolderFirstNamesLatin ?? string.Empty),
                new XElement("cardHolderPreferredLanguage", holder.CardHolderPreferredLanguage ?? string.Empty)
            );

            if (holder is DriverCardHolder driverHolder)
            {
                cardHolderElement.Add(
                    new XElement("cardHolderBirthDate", driverHolder.CardHolderBirthDate.ToString("yyyy-MM-dd")),
                    new XElement("cardHolderAddressAzari", driverHolder.CardHolderAddressAzari ?? string.Empty),
                    new XElement("cardHolderAddressLatin", driverHolder.CardHolderAddressLatin ?? string.Empty)
                );
            }

            if (holder is WorkshopCardHolder workshopHolder)
            {
                cardHolderElement.Add(
                    new XElement("cardHolderAddressAzari", workshopHolder.CardHolderAddressAzari ?? string.Empty),
                    new XElement("cardHolderAddressLatin", workshopHolder.CardHolderAddressLatin ?? string.Empty)
                );
            }

            cardElements.Add(cardHolderElement);
        }

        private void AddCardIssuingAuthorityIfExists(List<XElement> cardElements, ExportModelCardIssuingAuthority authority)
        {
            if (authority == null) return;

            cardElements.Add(new XElement("cardIssuingAuthority",
                new XElement("cardIssuingAuthorityNameAzari", authority.CardIssuingAuthorityNameAzari ?? string.Empty),
                new XElement("cardIssuingAuthorityNameLatin", authority.CardIssuingAuthorityNameLatin ?? string.Empty),
                new XElement("cardIssuingAuthorityAddressAzari", authority.CardIssuingAuthorityAddressAzari ?? string.Empty),
                new XElement("cardIssuingAuthorityAddressLatin", authority.CardIssuingAuthorityAddressLatin ?? string.Empty)
            ));
        }

        private void AddControlBodyIfExists(List<XElement> cardElements, ExportModelInspectionAgency controlBody)
        {
            if (controlBody == null) return;

            cardElements.Add(new XElement("controlBody",
                new XElement("controlBodyNameAzari", controlBody.ControlBodyNameAzari ?? string.Empty),
                new XElement("controlBodyNameLatin", controlBody.ControlBodyNameLatin ?? string.Empty),
                new XElement("controlBodyAddressAzari", controlBody.ControlBodyAddressAzari ?? string.Empty),
                new XElement("controlBodyAddressLatin", controlBody.ControlBodyAddressLatin ?? string.Empty)
            ));
        }

        private void AddDrivingLicenceInfoIfExists(List<XElement> cardElements, DrivingLicenceInfo drivingLicenceInfo)
        {
            if (drivingLicenceInfo == null) return;

            cardElements.Add(new XElement("drivingLicenceInfo",
                new XElement("drivingLicenceNumber", drivingLicenceInfo.DrivingLicenceNumber ?? string.Empty),
                new XElement("drivingLicenceIssuingAuthorityAzari", drivingLicenceInfo.DrivingLicenceIssuingAuthorityAzari ?? string.Empty),
                new XElement("drivingLicenceIssuingAuthorityLatin", drivingLicenceInfo.DrivingLicenceIssuingAuthorityLatin ?? string.Empty),
                new XElement("drivingLicenceIssuingNation", drivingLicenceInfo.DrivingLicenceIssuingNation)
            ));
        }

        private void AddBinaryDataIfExists(List<XElement> cardElements, ExportModelBinaryData binaryData)
        {
            if (binaryData == null) return;

            cardElements.Add(new XElement("binaryData",
                new XElement("photograph", binaryData.Photograph ?? string.Empty),
                new XElement("signature", binaryData.Signature ?? string.Empty),
                new XElement("photographMinioToken", binaryData.PhotographMinioToken ?? string.Empty),
                new XElement("signatureMinioToken", binaryData.SignatureMinioToken ?? string.Empty)
            ));
        }

        private void AddCompanyIfExists(List<XElement> cardElements, ExportModelTransporter company)
        {
            if (company == null) return;

            cardElements.Add(new XElement("company",
                new XElement("companyNameAzari", company.CompanyNameAzari ?? string.Empty),
                new XElement("companyNameLatin", company.CompanyNameLatin ?? string.Empty),
                new XElement("companyAddressAzari", company.CompanyAddressAzari ?? string.Empty),
                new XElement("companyAddressLatin", company.CompanyAddressLatin ?? string.Empty)
            ));
        }

        private void AddWorkshopIfExists(List<XElement> cardElements, ExportModelWorkshop workshop)
        {
            if (workshop == null) return;

            cardElements.Add(new XElement("workshop",
                new XElement("workshopNameAzari", workshop.WorkshopNameAzari ?? string.Empty),
                new XElement("workshopNameLatin", workshop.WorkshopNameLatin ?? string.Empty),
                new XElement("workshopAddressAzari", workshop.WorkshopAddressAzari ?? string.Empty),
                new XElement("workshopAddressLatin", workshop.WorkshopAddressLatin ?? string.Empty)
            ));
        }

        private CardExportWrapper ResolveJobInformation(ManufactureApiResponse response, int totalCards)
        {
            var jobInformation = ExtractJobInformationFromWrappers(response) ?? new CardExportWrapper();

            // Default dəyərləri təyin et
            jobInformation.CardAmount = totalCards;
            jobInformation.CreationTime = jobInformation.CreationTime == default ? DateTime.Now : jobInformation.CreationTime;
            jobInformation.CardIssuingMemberState = string.IsNullOrWhiteSpace(jobInformation.CardIssuingMemberState)
                ? "AZ"
                : jobInformation.CardIssuingMemberState;
            jobInformation.JobNumber = jobInformation.JobNumber;
            jobInformation.EquipmentType = jobInformation.EquipmentType;

            return jobInformation;
        }

        private CardExportWrapper ExtractJobInformationFromWrappers(ManufactureApiResponse response)
        {
            // Əvvəlcə bütün wrapper-lardan məlumatları yığırıq
            var wrappers = new CardExportWrapper[]
            {
                response.DriverCardsExportModel,
                response.TransporterCardsExportModel,
                response.WorkshopCardsExportModel,
                response.InspectorCardsExportModel
            }.Where(w => w != null).ToList();

            if (wrappers.Count == 0)
                return null;

            // İlk wrapper-dan məlumatları götürürük
            var firstWrapper = wrappers.First();

            return new CardExportWrapper
            {
                JobNumber = firstWrapper.JobNumber,
                CardIssuingMemberState = firstWrapper.CardIssuingMemberState,
                CardAmount = firstWrapper.CardAmount,
                CreationTime = firstWrapper.CreationTime,
                EquipmentType = firstWrapper.EquipmentType
            };
        }

        private string FormatDateTime(DateTime value)
        {
            return value == default
                ? string.Empty
                : value.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        //private static int _lastJobNumber = 0;
        //private static readonly object _lock = new object();

        //private string GetNextJobNumber()
        //{
        //    lock (_lock)
        //    {
        //        _lastJobNumber++;
        //        return _lastJobNumber.ToString();
        //    }
        //}
    }
}