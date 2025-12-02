using Application.External.Taxograf.Models;

namespace Application.Converters
{
    /// <summary>
    /// ManufactureApiResponse-ni controlCardRequest XML formatına çevirən converter interface
    /// </summary>
    public interface IControlCardRequestXmlConverter
    {
        /// <summary>
        /// ManufactureApiResponse-ni controlCardRequest XML formatına çevirir
        /// </summary>
        string ConvertToXml(ManufactureApiResponse response);
    }
}

