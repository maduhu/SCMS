using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.FleetIncidences
{
   public interface IFleetIncidenceService
    {
       bool IsIncidenceSaved(FleetMajorIncidence entity);
       bool IsFleetIncidenceEdited(FleetMajorIncidence entity);
       FleetMajorIncidence GetFleetIncidenceById(Guid incidenceId);
       bool IsFleetIncidenceDeleted(Guid incidenceId);
       List<FleetMajorIncidence> GetFleetIncidencs(Guid cpId);
    }
}
