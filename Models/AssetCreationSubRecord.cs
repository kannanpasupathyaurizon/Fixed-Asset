using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetCreationSubRecord
    {
        [DataMember]
        public string AssetNumber;

        [DataMember]
        public string AssetClass;

        [DataMember]
        public string CompanyCode;

        [DataMember]
        public string Description;

        [DataMember]
        public string PlateNo;

        [DataMember]
        public string InventNo;

        [DataMember]
        public string Manufacturer;

        [DataMember]
        public string Note;

        [DataMember]
        public string SerialNo;

        [DataMember]
        public string InitialAcq;

        [DataMember]
        public string CostCenter;

        [DataMember]
        public string InternalOrd;

        [DataMember]
        public string Location;

        [DataMember]
        public string Quantity;

        [DataMember]
        public string BaseOum;

        [DataMember]
        public string Description2;

        [DataMember]
        public string Room;

        [DataMember]
        public string CustFields1;

        [DataMember]
        public string CustFields3;

        [DataMember]
        public string ZZ_TPLNR;

        [DataMember]
        public string ZZ_EQUNR;

        [DataMember]
        public string ZZ_PSPNR;

        [DataMember]
        public string ULifeYears;

        [DataMember]
        public string ULifePrds;
        

        [DataMember]
        public ResponseMessage ResponseMessage;



        public AssetCreationSubRecord(
            string assetClass,
            string companyCode,
            string description,
            string plateNo,
            string inventNo,
            string manufacturer,
            string note,
            string serialNo,
            string initialAcq,
            string costCenter,
            string internalOrd,
            string location,
            string quantity,
            string baseOum,
            string description2,
            string room,
             string custFields1,
            string custFields3,
            string zz_tplnr,
            string zz_equnr,
            string zz_pspnr,
            string ulifeYears,
            string ulifePrds,
            string assetNumber,
            ResponseMessage responseMessage)
        {
            this.AssetClass = assetClass;
            this.CompanyCode = companyCode;
            this.Description = description;
            this.PlateNo = plateNo;
            this.InventNo = inventNo;
            this.Manufacturer = manufacturer;
            this.Note = note;
            this.SerialNo = serialNo;
            this.InitialAcq = initialAcq;
            this.CostCenter = costCenter;
            this.InternalOrd = internalOrd;
            this.Location = location;
            this.Quantity = quantity;
            this.BaseOum = baseOum;
            this.Description2 = description2;
            this.Room = room;
            this.CustFields1 = custFields1;
            this.CustFields3 = custFields3;
            this.ZZ_TPLNR = zz_tplnr;
            this.ZZ_EQUNR = zz_equnr;
            this.ZZ_PSPNR = zz_pspnr;
            this.ULifeYears = ulifeYears;
            this.ULifePrds = ulifePrds;
            this.AssetNumber = assetNumber;

            this.ResponseMessage = responseMessage;
        }
    }
}