using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ITL.IHR.Factory
{
    public class TicketFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdTicket";
        private readonly string UpdateSPName = "usp_InsUpdTicket";
        private readonly string DeleteSPName = "USP_DeleteTicket";
        private readonly string SelectSPName = "USP_GetTicket";
        #endregion

        public TicketFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            Ticket obj1 = obj as Ticket;
            base.parms.Add("@RequestedByID", obj1.RequestedByID);
            base.parms.Add("@AssignedToID", obj1.AssignedToID);
            base.getStoredProc = SelectSPName;
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            base.parms.Add("@TicketID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }

        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@TicketID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }

        public override Response<T> Delete<T>(T obj)
        {
            Ticket obj1 = obj as Ticket;
            base.parms.Add("@TicketID", obj1.TicketID);
           
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            
            Ticket obj1 = obj as Ticket;
          

            base.parms.Add("@TicketID", obj1.TicketID);
            base.parms.Add("@TicketTypeID", obj1.TicketTypeID);
            base.parms.Add("@RequestedByID", obj1.RequestedByID);
            base.parms.Add("@Description", obj1.Description);
            base.parms.Add("@ModuleID", obj1.ModuleID);
            base.parms.Add("@ID", obj1.ID);
            base.parms.Add("@AssignedToID", obj1.AssignedToID);
            base.parms.Add("@StatusID", obj1.StatusID);
            base.parms.Add("@ResolvedDate", obj1.ResolvedDate);
            base.parms.Add("@Comment", obj1.Comment);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@Title", obj1.Title);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            Ticket Ticket = new Ticket();
            Ticket = reader.Read<Ticket>().FirstOrDefault();
            return (T)Convert.ChangeType(Ticket, typeof(T));
        }

        public int Compare(AbstractDataObject x, AbstractDataObject y)
        {
            return 0;
        }
         
    }
}