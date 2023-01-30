using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Dapper;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.Extensions.Configuration;

namespace ITL.IHR.Factory
{
    public class EmployeeSkillFactory : AbstractFactory1
    {

        #region ProcedureNames
        private readonly string InsertSPName = "usp_InsUpdEmployeeSkill";
        private readonly string UpdateSPName = "usp_InsUpdEmployeeSkill";
        private readonly string DeleteSPName = "usp_DeleteEmployeeSkill";
        private readonly string SelectSPName = "usp_GetEmployeeSkill";
        #endregion

        public EmployeeSkillFactory(string connString, IConfiguration config)
            : base(connString, config)
        {
        }

        public override Response<List<T>> GetList<T>(T obj)
        {
            EmployeeSkill obj1 = obj as EmployeeSkill;
            base.getStoredProc = SelectSPName;
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@SkillTypeID", obj1.SkillTypeID);
            base.parms.Add("@Skill", obj1.Skill);
            return base.GetList<T>();
        }

        public override Response<T> GetByID<T>(T obj)
        {
            EmployeeSkill obj1 = obj as EmployeeSkill;
            base.parms.Add("@EmployeeSkillID", obj1.EmployeeSkillID);
            base.getStoredProc = SelectSPName;
            return base.GetByID<T>();
        }


        public override Response<T> GetRelatedObjectsByID<T>(T obj)
        {
            base.parms.Add("@EmployeeSkillID", (obj as AbstractDataObject).RecordID);
            base.getStoredProc = SelectSPName;
            return base.GetRelatedObjectsByID<T>();
        }


        public override Response<T> Delete<T>(T obj)
        {
            EmployeeSkill obj1 = new EmployeeSkill();
            obj1 = obj as EmployeeSkill;
            base.parms.Add("@EmployeeSkillID", obj1.EmployeeSkillID);
            base.selectStoredProc = SelectSPName;
            base.getStoredProc = DeleteSPName;

            return base.DeleteInstance<T>();
        }

        public override Response<T> Save<T>(T obj)
        {
            EmployeeSkill obj1 = obj as EmployeeSkill;
            base.parms.Add("@EmployeeSkillID", obj1.EmployeeSkillID);
            base.parms.Add("@EmployeeID", obj1.EmployeeID);
            base.parms.Add("@SkillTypeID", obj1.SkillTypeID);
            base.parms.Add("@Skill", obj1.Skill);
            base.parms.Add("@Experience", obj1.Experience);
            base.parms.Add("@CreatedBy", obj1.CreatedBy);
            base.parms.Add("@ModifiedBy", obj1.ModifiedBy);
            base.parms.Add("@ReturnCode", obj1.RecordID, direction: ParameterDirection.Output, size: sizeof(int));
            base.getStoredProc = InsertSPName;
            base.selectStoredProc = SelectSPName;
            return base.SaveInstanceReturnOutput<T>(obj);
        }

        protected override T LoadRelatedObjects<T>(SqlMapper.GridReader reader)
        {
            EmployeeSkill employeeSkill = new EmployeeSkill();
            return (T)Convert.ChangeType(employeeSkill, typeof(T));
        }

    }
}