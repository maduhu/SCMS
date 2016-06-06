using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.FleetAcceories
{
    public interface IFleetAccesoriesService
    {
        bool IsFleetAccesorySaved(FleetEquipment entity);
        bool IsFleetAccesoryEdited(FleetEquipment entity);
        FleetEquipment GetFleetAccesoryById(Guid accesoryId);
        List<FleetEquipment> GetFleetAccesories(Guid cpId);
        bool IsFleetAccesoryDeleted(Guid accesoryId);
    }
}
