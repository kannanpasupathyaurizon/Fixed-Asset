using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetAcquisitionRecord
    {
        [DataMember]
        public string Class;
        
        [DataMember]
        public string CompanyCode;

        [DataMember]
        public string AssetDesc;

        [DataMember]
        public string LicensePlateNo;

        [DataMember]
        public string KMPoint;

        [DataMember]
        public string Manufacturer;

        [DataMember]
        public string Model;

        [DataMember]
        public string SerialNo;

        [DataMember]
        public string AcquisitionDate;

        [DataMember]
        public string CostCenter;

        [DataMember]
        public string InternalOrder;

        [DataMember]
        public string Location;

        [DataMember]
        public string Quantity;

        [DataMember]
        public string QuantityUnit;

        [DataMember]
        public string AdditionalAssetDesc1;

        [DataMember]
        public string AdditionalDesc2;

        [DataMember]
        public string CustomField1;

        [DataMember]
        public string CustomField3;

        [DataMember]
        public string FunctionalLoc;

        [DataMember]
        public string Equipment;

        [DataMember]
        public string Project;

        [DataMember]
        public string AssetBooks;

        [DataMember]
        public string UsefulLifeYears;

        [DataMember]
        public string UsefulLifeMonths;

        [DataMember]
        public ResponseMessage ResponseMessage;

        

        public AssetAcquisitionRecord(
            string classs,
            string companyCode,
            string assetDesc,
            string licensePlateNo,
            string kmPoint,
            string manufacturer,
            string model,
            string serialNo,
            string acquisitionDate,
            string costCenter,
            string internalOrder,
            string location,
            string quantity,
            string quantityUnit,
            string additionalAssetDesc1,
            string additionalDesc2,
            string customField1,
            string customField3,
            string functionalLoc,
            string equipment,
            string project,
            string assetBooks,
            string usefulLifeYears,
            string usefulLifeMonths,
            ResponseMessage responseMessage)
        {
            this.Class = classs;
            this.CompanyCode = companyCode;
            this.AssetDesc = assetDesc;
            this.LicensePlateNo = licensePlateNo;
            this.KMPoint = kmPoint;
            this.Manufacturer = manufacturer;            
            this.Model = model;
            this.SerialNo = serialNo;
            this.AcquisitionDate = acquisitionDate;
            this.CostCenter = costCenter;
            this.InternalOrder = internalOrder;
            this.Location = location;
            this.Quantity = quantity;
            this.QuantityUnit = quantityUnit;
            this.AdditionalAssetDesc1 = additionalAssetDesc1;
            this.AdditionalDesc2 = additionalDesc2;
            this.CustomField1 = customField1;
            this.CustomField3 = customField3;
            this.FunctionalLoc = functionalLoc;
            this.Equipment = equipment;
            this.Project = project;
            this.AssetBooks = assetBooks;
            this.UsefulLifeYears = usefulLifeYears;
            this.UsefulLifeMonths = usefulLifeMonths;
            this.ResponseMessage = responseMessage;
        }
    }
}