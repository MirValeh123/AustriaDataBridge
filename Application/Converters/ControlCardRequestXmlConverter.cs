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
            var xmlDocument = BuildXmlDocument(response, totalCards);

            return FormatXmlDocument(xmlDocument);
        }

        private int CalculateTotalCards(ManufactureApiResponse response)
        {
            return (response.DriverCardsExportModel?.Count ?? 0) +
                   (response.TransporterCardsExportModel?.Count ?? 0) +
                   (response.WorkshopCardsExportModel?.Count ?? 0) +
                   (response.InspectorCardsExportModel?.Count ?? 0);
        }

        private XDocument BuildXmlDocument(ManufactureApiResponse response, int totalCards)
        {
            var root = new XElement("controlCardRequest",
                new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                
                new XElement("data",
                    BuildJobInformationElement(totalCards),
                    BuildCardRequestElement(response)
                )
            );

            return new XDocument(new XDeclaration("1.0", "utf-8", null), root);
        }

        private XElement BuildJobInformationElement(int totalCards)
        {
            return new XElement("jobInformation",
                new XElement("jobNumber", "100004085"),
                new XElement("creationTime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("equipmentType", 3),
                new XElement("cardAmount", totalCards),
                new XElement("cardIssuingMemberState", "AZ")
            );
        }

        private XElement BuildCardRequestElement(ManufactureApiResponse response)
        {
            var cardElements = new List<XElement>();

            if (response.DriverCardsExportModel != null)
                cardElements.AddRange(response.DriverCardsExportModel.Select(CreateDriverCardElement));

            if (response.TransporterCardsExportModel != null)
                cardElements.AddRange(response.TransporterCardsExportModel.Select(CreateTransporterCardElement));

            if (response.WorkshopCardsExportModel != null)
                cardElements.AddRange(response.WorkshopCardsExportModel.Select(CreateWorkshopCardElement));

            if (response.InspectorCardsExportModel != null)
                cardElements.AddRange(response.InspectorCardsExportModel.Select(CreateInspectorCardElement));

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
                // Driver kartları üçün də ümumi header hissəsini saxlayırıq
                new XElement("Test", 1),
                new XElement("cardNumber", string.Empty),
                new XElement("cardIssueDate", DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("cardValidityBegin", DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("cardExpiryDate", DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"))
            };

            AddCardHolderIfExists(cardElements, dto.CardHolder);
            AddCardIssuingAuthorityIfExists(cardElements, dto.CardIssuingAuthority);
            AddDrivingLicenceInfoIfExists(cardElements, dto.DrivingLicenceInfo);
            AddBinaryDataIfExists(cardElements, dto.BinaryData);

            return new XElement("card", cardElements);
        }

        private XElement CreateTransporterCardElement(TrasnporterCardExportDto dto)
        {
            var cardElements = new List<XElement>
            {
                new XElement("Test", dto.Test),
                new XElement("cardNumber", dto.CardNumber ?? string.Empty),
                new XElement("cardIssueDate", dto.CardIssueDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("cardValidityBegin", dto.CardValidityBegin.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("cardExpiryDate", dto.CardExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss"))
            };

            AddCardHolderIfExists(cardElements, dto.CardHolder);
            AddCardIssuingAuthorityIfExists(cardElements, dto.CardIssuingAuthority);

            return new XElement("card", cardElements);
        }

        private XElement CreateWorkshopCardElement(WorkShopCardExportDto dto)
        {
            var cardElements = new List<XElement>
            {
                new XElement("Test", dto.Test),
                new XElement("cardNumber", dto.CardNumber ?? string.Empty),
                new XElement("cardIssueDate", dto.CardIssueDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("cardValidityBegin", dto.CardValidityBegin.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("cardExpiryDate", dto.CardExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss"))
            };

            AddCardHolderIfExists(cardElements, dto.CardHolder);
            AddCardIssuingAuthorityIfExists(cardElements, dto.CardIssuingAuthority);

            return new XElement("card", cardElements);
        }

        private XElement CreateInspectorCardElement(InspectorCardExportDto dto)
        {
            var cardElements = new List<XElement>
            {
                new XElement("Test", dto.Test),
                new XElement("cardNumber", dto.CardNumber ?? string.Empty),
                new XElement("cardIssueDate", dto.CardIssueDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("cardValidityBegin", dto.CardValidityBegin.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement("cardExpiryDate", dto.CardExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss"))
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

            // Driver kartları üçün əlavə field-ləri də yazırıq
            if (holder is DriverCardHolder driverHolder)
            {
                cardHolderElement.Add(
                    new XElement("cardHolderBirthDate", driverHolder.CardHolderBirthDate.ToString("yyyy-MM-dd")),
                    new XElement("cardHolderAddressAzari", driverHolder.CardHolderAddressAzari ?? string.Empty),
                    new XElement("cardHolderAddressLatin", driverHolder.CardHolderAddressLatin ?? string.Empty)
                );
            }

            // Workshop kartları üçün ünvan field-ləri
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
                new XElement("signature", binaryData.Signature ?? string.Empty)
            ));
        }
    }
}

