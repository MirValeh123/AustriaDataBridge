using System;
using System.Collections.Generic;
using Application.External.Taxograf.Models.Dtos;
using Domain.Enums;

namespace Application.External.Taxograf.Models
{
    public class ManufactureApiResponse
    {
        public DriverCardExportWrapper DriverCardsExportModel { get; set; }
        public TransporterCardExportWrapper TransporterCardsExportModel { get; set; }
        public WorkshopCardExportWrapper WorkshopCardsExportModel { get; set; }
        public InspectorCardExportWrapper InspectorCardsExportModel { get; set; }
    }

    public class CardExportWrapper
    {
        private string _jobNumber;

        public string JobNumber
        {
            get => _jobNumber ??= GetNextJobNumber();
            set => _jobNumber = value;
        }

        public DateTime CreationTime { get; set; }
        public EquipmentTypeEnum EquipmentType { get; set; }
        public int CardAmount { get; set; }
        public string CardIssuingMemberState { get; set; }

        private static int _lastJobNumber = 0;
        private static readonly object _lock = new object();

        private static string GetNextJobNumber()
        {
            lock (_lock)
            {
                _lastJobNumber++;
                return _lastJobNumber.ToString();
            }
        }
    }

    public class DriverCardExportWrapper : CardExportWrapper
    {
        public List<DriverCardExportDto> ExportModels { get; set; }
    }

    public class TransporterCardExportWrapper : CardExportWrapper
    {
        public List<TransporterCardExportDto> ExportModels { get; set; }
    }

    public class WorkshopCardExportWrapper : CardExportWrapper
    {
        public List<WorkShopCardExportDto> ExportModels { get; set; }
    }

    public class InspectorCardExportWrapper : CardExportWrapper
    {
        public List<InspectorCardExportDto> ExportModels { get; set; }
    }



   
}

