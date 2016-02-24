using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Services;
using Newtonsoft.Json;

namespace RestServiceForFlightSearch
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRestServiceImp" in both code and config file together.
    [ServiceContract]
    public interface IRestServiceImp
    {
        [OperationContract]
        [WebGet(UriTemplate = "json/{cityName}")]
        string GetCityNames(string cityName);
    }
}
