using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Vehicle.DataAccessLayer.Entities;
using Vehicle.DataAccessLayer.Utils;
using System.Linq;

namespace Vehicle.DataAccessLayer.Repository
{
    public interface IVehicleRepository : IBaseRepository<int, VehicleEntity, VehicleDetailEntity, VehicleFilterEntity, VehicleLovEntity>
    {

    }
    public class VehicleRepository : IVehicleRepository
    {
        private readonly IDataAccess _dataAccess;
        public VehicleRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }

        public async Task<int> Create(VehicleEntity entity)
        {
            var sql = "dbo.DEF_Vehicle_Create";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("VehicleID", dbType: DbType.Int32, direction: ParameterDirection.Output);
            sqlParams.Add("Name", entity.Name);
            sqlParams.Add("ModelID", entity.ModelID);
            sqlParams.Add("Active", entity.Active);

            try
            {
                using (var con = _dataAccess.CreateConnection())
                {
                    await con.ExecuteAsync(
                        sql: sql,
                        param: sqlParams,
                        commandType: CommandType.StoredProcedure
                    );
                    var vehicleId = sqlParams.Get<int>("VehicleID");
                    return vehicleId;
                }
            }
            catch (SqlException ex)
            {
                // Detaylı hata mesajını yakalayıp loglama
                var errorMessage = $"SQL Error: {ex.Message} \nError Number: {ex.Number} \nState: {ex.State} \nClass: {ex.Class}";
                throw new Exception(errorMessage, ex);
                // hata almamın sebebi de Vehicle Create yapmak isterken ModelID 'yi yanlış vermem olmuş.
            }
        }


        public async Task Delete(int id)
        {
            var sql = "dbo.DEF_Vehicle_Delete";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("VehicleID", id);

            try
            {
                using (var con = _dataAccess.CreateConnection())
                {
                    await con.ExecuteAsync(
                        sql: sql,
                        param: sqlParams,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<VehicleDetailEntity> Read(int id)
        {
            var sql = "dbo.DEF_Vehicle_Read";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("VehicleID", id);
            try
            {
                using (var con = _dataAccess.CreateConnection())
                {
                    var result = await con.QueryAsync<VehicleDetailEntity>
                    (
                        sql: sql,
                        param: sqlParams,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.SingleOrDefault();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        //FILTER
        public async Task<List<VehicleDetailEntity>> ReadAll(VehicleFilterEntity filter)
        {
            var sql = "dbo.DEF_Vehicle_ReadAll";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("BrandID", filter.BrandId);
            sqlParams.Add("ModelYear", filter.ModelYear);
            try
            {
                using (var con = _dataAccess.CreateConnection())
                {
                    var result = await con.QueryAsync<VehicleDetailEntity>
                    (
                        sql: sql,
                        param: sqlParams,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.AsList();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<VehicleLovEntity>> ReadLov()
        {
            var sql = "dbo.DEF_Vehicle_Lov";
            var sqlParams = new DynamicParameters();
            try
            {
                using (var con = _dataAccess.CreateConnection())
                {
                    var result = await con.QueryAsync<VehicleLovEntity>
                    (
                        sql: sql,
                        param: sqlParams,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.AsList();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task Update(VehicleEntity entity)
        {
            var sql = "dbo.DEF_Vehicle_Update";
            var sqlParams = new DynamicParameters();
            sqlParams.Add("VehicleID", entity.VehicleId);
            sqlParams.Add("Name", entity.Name);
            sqlParams.Add("ModelID", entity.ModelID);
            sqlParams.Add("Active", entity.Active);

            try
            {
                using (var con = _dataAccess.CreateConnection())
                {
                    await con.ExecuteAsync(
                        sql: sql,
                        param: sqlParams,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
