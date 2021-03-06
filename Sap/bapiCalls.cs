﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using FixedAsset.Models;
using ERPConnect;
using Serilog;

namespace FixedAsset.Sap
{
    public class bapiCalls
    {
        public void SapFixedAssetSearch(
            R3Connection r3Connection,
            FixedAssetSearch fixedAssetSearch)
        {
            fixedAssetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);
            int counter = 0;
            try
            {
                // Create a Bapi object, fill parameters and execute
                var bapiCall = r3Connection.CreateFunction("ZBAPI_FIXED_ASSET_DETAILS");
                var bapiName = "ZBAPI_FIXED_ASSET_DETAILS";
                Log.Information("Calling BAPI: " + bapiName);

                bapiCall.Exports["IV_COMPANYCODE"].ParamValue = fixedAssetSearch.CompanyCode == null
                    ? ""
                    : fixedAssetSearch.CompanyCode.PadLeft(4, '0');
                bapiCall.Exports["IV_MAINASSET"].ParamValue =
                    fixedAssetSearch.Asset == null ? "" : fixedAssetSearch.Asset.PadLeft(12, '0');
                bapiCall.Exports["IV_ASSETCLASS"].ParamValue = fixedAssetSearch.AssetClass == null
                    ? ""
                    : fixedAssetSearch.AssetClass.PadLeft(8, '0');
                bapiCall.Exports["IV_ASSETDESCRPT"].ParamValue = fixedAssetSearch.AssetDescription ?? "";
                bapiCall.Exports["IV_COSTCENTRE"].ParamValue = fixedAssetSearch.CostCentre == null
                    ? ""
                    : fixedAssetSearch.CostCentre.PadLeft(10, '0');
                bapiCall.Exports["IV_LIMIT"].ParamValue = fixedAssetSearch.SearchResultLimit.ToString();
                bapiCall.Execute();

                // Read the response table

                var resultTable = bapiCall.Tables["ET_DETAILS"].ToADOTable();
                fixedAssetSearch.SearchResults = new List<FixedAssetSearchResult>();

                foreach (DataRow currentRow in resultTable.Rows)
                {
                    if (fixedAssetSearch.SearchResultLimit != null &&
                        counter == fixedAssetSearch.SearchResultLimit)
                    {
                        break;
                    }
                    decimal accountingBookCost;
                    decimal accountingBookWrittenDown;

                    decimal.TryParse(currentRow["ACCBKCOST"].ToString(), out accountingBookCost);
                    decimal.TryParse(currentRow["ACCBKWDV"].ToString(), out accountingBookWrittenDown);

                    fixedAssetSearch
                        .SearchResults
                        .Add(
                            new FixedAssetSearchResult(
                                currentRow["COMPANY_CODE"].ToString(),
                                currentRow["ASSET_NO"].ToString(),
                                currentRow["SUB_ASSET_NO"].ToString(),
                                currentRow["ASSET_CLASS"].ToString(),
                                currentRow["ASSET_DESC"].ToString(),
                                currentRow["SAP_EQUIP_NO"].ToString(),
                                currentRow["COST_CENTRE"].ToString(),
                                currentRow["INTERNAL_ORDER"].ToString(),                                
                                currentRow["FUNC_LOC"].ToString(),
                                currentRow["LOCATION"].ToString(),
                                accountingBookCost,
                                accountingBookWrittenDown
                            )
                        );
                    counter++;
                }
                Log.Information("Added Asset Disposal LookUp Data to List Successfully ");
                fixedAssetSearch.ResponseMessage.responseCode = HttpStatusCode.OK;
            }
            catch (Exception exception)
            {

                var message = "Error connecting to SAP";
                Log.Error(exception, message);
                fixedAssetSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                fixedAssetSearch.ResponseMessage.responseMessages = message;
            }
        }
        public void MiscFixedAssetSearch(
         R3Connection r3Connection,
         MiscFixedAssetSearch miscFixedAssetSearch)
        {
            miscFixedAssetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);
            int counter = 0;
            try
            {
                // Create a Bapi object, fill parameters and execute
                var bapiCall = r3Connection.CreateFunction("ZBAPI_LOC_COST_ORDER_LOOKUP");
                var bapiName = "ZBAPI_LOC_COST_ORDER_LOOKUP";

                Log.Information("Calling BAPI: " + bapiName);
                RFCStructure ivLocation = bapiCall.Exports["IV_LOCATION"].ToStructure();
                RFCStructure ivCostCenter = bapiCall.Exports["IV_COST_CENTER"].ToStructure();
                RFCStructure ivIntOrder = bapiCall.Exports["IV_INT_ORDER"].ToStructure();
                ivLocation["LOCCODE"] = miscFixedAssetSearch.LocationCode == null
                    ? " "
                    : miscFixedAssetSearch.LocationCode;
                ivLocation["LOCDESC"] = miscFixedAssetSearch.LocationDescription == null
                    ? " "
                    : miscFixedAssetSearch.LocationDescription;
                ivCostCenter["COSTCENTER"] = miscFixedAssetSearch.CostCentreCode == null
                    ? " "
                    : miscFixedAssetSearch.CostCentreCode;
                ivCostCenter["CC_DESC"] = miscFixedAssetSearch.CostCentreDescription == null
                    ? " "
                    : miscFixedAssetSearch.CostCentreDescription;
                ivCostCenter["COMPANY"] = miscFixedAssetSearch.CompanyCodeCC == null
                    ? " "
                    : miscFixedAssetSearch.CompanyCodeCC;
                ivIntOrder["ORDER"] = miscFixedAssetSearch.OrderCode == null
                    ? " "
                    : miscFixedAssetSearch.OrderCode;
                ivIntOrder["ORDER_DESC"] = miscFixedAssetSearch.OrderDescription == null
                    ? " "
                    : miscFixedAssetSearch.OrderDescription;
                ivIntOrder["COMPANY"] = miscFixedAssetSearch.CompanyCodeOrder == null
                    ? " "
                    : miscFixedAssetSearch.CompanyCodeOrder;
                ivIntOrder["ORDER_TYPE"] = miscFixedAssetSearch.OrderType == null
                    ? " "
                    : miscFixedAssetSearch.OrderType;

                if (Convert.ToInt32(miscFixedAssetSearch.SearchResultLimit) > 5000)
                {
                    miscFixedAssetSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                    miscFixedAssetSearch.ResponseMessage.responseMessages = "Please Enter Limit Less Than 5000";
                }
                else
                {
                    bapiCall.Exports["IV_LIMIT"].ParamValue = miscFixedAssetSearch.SearchResultLimit.ToString() == null
                    ? " "
                    : miscFixedAssetSearch.SearchResultLimit.ToString();

                    try
                    {

                        bapiCall.Execute();
                    }
                    catch (ABAPRuntimeException apAbapRuntimeException)
                    {
                        if (apAbapRuntimeException.ABAPException == "RFC_ERROR_SYSTEM_FAILURE")
                        {
                            miscFixedAssetSearch.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
                            miscFixedAssetSearch.ResponseMessage.responseMessages = apAbapRuntimeException.Description;

                        }
                        else
                        {
                            throw new Exception($"Unhandled ABAP Exception: {bapiName}", apAbapRuntimeException);
                        }
                    }
                    // Read the response table

                    var resultTable = bapiCall.Tables["ET_RESULT"].ToADOTable();
                    if (resultTable.Rows.Count == 0)
                    {
                        miscFixedAssetSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                        miscFixedAssetSearch.ResponseMessage.responseMessages = "No Records Found";

                    }
                    else
                    {
                        miscFixedAssetSearch.SearchResults = new List<MiscFixedAssetSearchResult>();

                        foreach (DataRow currentRow in resultTable.Rows)
                        {
                            if (miscFixedAssetSearch.SearchResultLimit != null &&
                                counter == miscFixedAssetSearch.SearchResultLimit)
                            {
                                break;
                            }

                            // Adding fields to Misc Lookup Search
                            String validFromDate = (String)(currentRow["VALIDFROM"]);
                            String validToDate = (String)(currentRow["VALIDTO"]);
                            string validToDateOut;
                            string validFromDateOut;
                            if (validFromDate == "00000000")
                            {
                                validFromDateOut = " ";
                            }
                            else
                            {
                                validFromDateOut = DateTime.ParseExact(validFromDate, "yyyyMMdd",
                                System.Globalization.CultureInfo.InvariantCulture).ToString("dd.MM.yyyy");
                            }
                            if (validToDate == "00000000")
                            {
                                validToDateOut = " ";
                            }
                            else
                            {
                                validToDateOut = DateTime.ParseExact(validToDate, "yyyyMMdd",
                            System.Globalization.CultureInfo.InvariantCulture).ToString("dd.MM.yyyy");
                            }
                            miscFixedAssetSearch
                                .SearchResults
                                .Add(
                                    new MiscFixedAssetSearchResult(
                                        currentRow["LOCCODE"].ToString(),
                                        currentRow["LOCDESC"].ToString(),
                                        currentRow["COSTCENTER"].ToString(),
                                        currentRow["CC_DESC"].ToString(),
                                        currentRow["COMPANY_COST"].ToString(),
                                        validFromDateOut.ToString(),
                                        validToDateOut.ToString(),
                                        currentRow["ORDER"].ToString(),
                                        currentRow["ORDER_TYPE"].ToString(),
                                        currentRow["ORDER_DESC"].ToString()
                                    )
                                );
                            counter++;
                        }
                        Log.Information("Added Misc LookUp Data to List Successfully ");
                        miscFixedAssetSearch.ResponseMessage.responseCode = HttpStatusCode.OK;
                    }
                }
                    
                
            }
            catch (Exception exception)
            {

                var message = "Error connecting to SAP";
                Log.Error(exception, message);

                miscFixedAssetSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                miscFixedAssetSearch.ResponseMessage.responseMessages = message;
            }
        }
        public void SapAssetSearch(
            R3Connection r3Connection,
            AssetSearch assetSearch)
        {
            assetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                // Create a Bapi object, fill parameters and execute
                var bapiCall = r3Connection.CreateFunction("BAPI_FIXEDASSET_GETDETAIL");
                var bapiName = "BAPI_FIXEDASSET_GETDETAIL";

                Log.Information("Calling BAPI: " + bapiName);
                DateTime evalDate = (DateTime)(assetSearch.EvaluationDate);

                bapiCall.Exports["COMPANYCODE"].ParamValue = assetSearch.CompanyCode.PadLeft(4, '0');
                bapiCall.Exports["ASSET"].ParamValue = assetSearch.Asset.PadLeft(12, '0');
                bapiCall.Exports["SUBNUMBER"].ParamValue = assetSearch.SubNumber.PadLeft(4, '0');
                bapiCall.Exports["EVALUATION_DATE"].ParamValue = evalDate.ToString("yyyyMMdd");
                try
                {
                    
                    bapiCall.Execute();

                }
                catch (ABAPRuntimeException apAbapRuntimeException)
                {
                    if (apAbapRuntimeException.ABAPException == "RFC_ERROR_SYSTEM_FAILURE")
                    {
                        assetSearch.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
                        assetSearch.ResponseMessage.responseMessages = apAbapRuntimeException.Description;

                    }
                    else
                    {
                        throw new Exception($"Unhandled ABAP Exception: {bapiName}", apAbapRuntimeException);
                    }
                }

                // Read the import structure RETURN to provide possible Messages
                RFCStructure BapiRet = bapiCall.Imports["RETURN"].ToStructure();
                var message = BapiRet["MESSAGE"].ToString();


                if (message == "")
                {
                    RFCStructure rfcBasicData = bapiCall.Imports["BASIC_DATA"].ToStructure();
                    RFCStructure rfcBapiOrgData = bapiCall.Imports["ORGANIZATIONAL_DATA"].ToStructure();
                    RFCStructure rfcBapiSpecialClassData = bapiCall.Imports["SPECIAL_CLASSIFICATIONS"].ToStructure();

                    assetSearch.SearchResults = new List<AssetSearchResult>();

                    assetSearch
                        .SearchResults
                        .Add(
                            new AssetSearchResult(
                                assetSearch.CompanyCode,
                                assetSearch.Asset,
                                rfcBasicData["DESCRIPT"].ToString(),
                                assetSearch.SubNumber,
                                rfcBapiOrgData["ASSETCLASS"].ToString(),
                                rfcBapiOrgData["COSTCENTER"].ToString()

                            )
                        );
                    Log.Information("Added Asset LookUp Data to List Successfully ");
                    assetSearch.ResponseMessage.responseCode = HttpStatusCode.OK;
                }
                else
                {
                    assetSearch.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
                    assetSearch.ResponseMessage.responseMessages = message;
                }
            }
            catch (Exception exception)
            {

                var message = "Error connecting to SAP";
                Log.Error(exception, message);

                assetSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                assetSearch.ResponseMessage.responseMessages = message;
            }
        }

        public void SapAssetDisposal(
            R3Connection r3Connection,
            AssetDisposalRequest assetDisposalRequest,
            bool validateOnly)
        {
            assetDisposalRequest.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);
            int counter = 1;
            try
            {
                // Create a Bapi object, fill parameters and execute
                var bapiBaseName = "ZBAPI_ASSET_RETIREMENT_";
                var bapiType = validateOnly ? "CHECK" : "POST";
                var bapiName = bapiBaseName + bapiType;
                RFCFunction bapiCall = null;
                Log.Information("Calling BAPI: " + bapiName);

                foreach (var currentRecord in assetDisposalRequest.DisposalRecords)
                {
                    bool responded = false;

                    if (bapiCall != null)
                    {
                        bapiCall = null;
                    }

                    bapiCall = r3Connection.CreateFunction(bapiName);
                    RFCStructure generalPostingData = bapiCall.Exports["GENERALPOSTINGDATA"].ToStructure();
                    RFCStructure retirementData = (RFCStructure)bapiCall.Exports["RETIREMENTDATA"].ToStructure();
                    RFCStructure furtherPostingData = (RFCStructure)bapiCall.Exports["FURTHERPOSTINGDATA"].ToStructure();
                    if (currentRecord.DocumentDate == null ||
                        currentRecord.PostingDate == null ||
                        currentRecord.ValueDate == null)
                    {
                        currentRecord.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
                        currentRecord.ResponseMessage.responseMessages = "Missing Date(s)";
                        continue;
                    }
                    DateTime documentDate = (DateTime)(currentRecord.DocumentDate);
                    DateTime postingDate = (DateTime)(currentRecord.PostingDate);
                    DateTime valueDate = (DateTime)(currentRecord.ValueDate);
                    generalPostingData["DOC_DATE"] = documentDate.AddHours(10).ToString("yyyyMMdd");
                    generalPostingData["PSTNG_DATE"] = postingDate.AddHours(10).ToString("yyyyMMdd");
                    generalPostingData["TRANS_DATE"] = valueDate.AddHours(10).ToString("yyyyMMdd");
                    generalPostingData["COMP_CODE"] = currentRecord.CompanyCode.PadLeft(4, '0');
                    generalPostingData["ASSETMAINO"] = currentRecord.Asset.PadLeft(12, '0');
                    generalPostingData["ASSETSUBNO"] = currentRecord.SubNumber.PadLeft(4, '0');
                    string text = Convert.ToString(currentRecord.Text);
                    string refernce = Convert.ToString(currentRecord.Reference);
                    if (text == "" || text == null)
                    {

                        furtherPostingData["ITEM_TEXT"] = "";
                    }
                    else
                    {
                        furtherPostingData["ITEM_TEXT"] = Convert.ToString(currentRecord.Text);
                    }
                    if (refernce == "" || refernce == null)
                    {

                        furtherPostingData["REF_DOC_NO"] = "";
                    }
                    else
                    {
                        furtherPostingData["REF_DOC_NO"] = Convert.ToString(currentRecord.Reference);
                    }
                    string partialCurrentYear = Convert.ToString(currentRecord.PartialCurrentYear);
                    string partialPriorYear = Convert.ToString(currentRecord.PartialPriorYear);
                    string proceedFromSales = Convert.ToString(currentRecord.ProceedFromSales);
                    string partialDisposalValue;
                    partialDisposalValue = Convert.ToString(currentRecord.PartialDisposal);
                    int myInt;
                    if (proceedFromSales != null)
                    {
                        if (partialCurrentYear != null && proceedFromSales == "0")
                        {
                            generalPostingData["ASSETTRTYP"] = "250";
                        }
                        if (partialCurrentYear != null && proceedFromSales != "0")
                        {
                            generalPostingData["ASSETTRTYP"] = "260";
                        }
                        if (partialPriorYear != null && proceedFromSales == "0")
                        {
                            generalPostingData["ASSETTRTYP"] = "200";
                        }
                        if (partialPriorYear != null && proceedFromSales != "0")
                        {
                            generalPostingData["ASSETTRTYP"] = "210";
                        }
                        if (partialCurrentYear == null && partialPriorYear == null)
                        {
                            retirementData["REV_ON_RET"] = proceedFromSales;
                        }
                    }
                    else
                    {
                        if (partialCurrentYear != null && proceedFromSales == null)
                        {
                            generalPostingData["ASSETTRTYP"] = "250";
                        }
                        if (partialPriorYear != null && proceedFromSales == null)
                        {
                            generalPostingData["ASSETTRTYP"] = "200";
                        }
                        if (partialCurrentYear == null && partialPriorYear == null)
                        {
                            retirementData["REV_ON_RET"] = proceedFromSales;
                        }
                    }
                    if (partialDisposalValue == null || partialDisposalValue == "")
                    {
                        // retirementData["AMOUNT"] = "";

                    }
                    else
                    {

                        bool isNumerical = int.TryParse(partialDisposalValue, out myInt);

                        if (isNumerical == true)
                        {
                            retirementData["AMOUNT"] = partialDisposalValue;
                        }
                        else
                        {
                            decimal value;
                            value = Convert.ToDecimal(partialDisposalValue);
                            if (value < 1)
                            {
                                retirementData["PERC_RATE"] = value * 100;
                            }
                            else
                            {
                                retirementData["AMOUNT"] = value;
                            }
                        }
                    }

                    retirementData["REV_ON_RET"] = proceedFromSales;
                    retirementData["VALUEDATE"] = valueDate.AddHours(10).ToString("yyyyMMdd");

                    try
                    {
                        bapiCall.Execute();
                    }
                    catch (ABAPRuntimeException apAbapRuntimeException)
                    {
                        if (apAbapRuntimeException.ABAPException == "RFC_ERROR_SYSTEM_FAILURE")
                        {
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
                            currentRecord.ResponseMessage.responseMessages = apAbapRuntimeException.Description;
                            responded = true;
                            Log.Error("Row No:" + counter + "  " + currentRecord.ResponseMessage.responseMessages);
                        }
                        else
                        {
                            throw new Exception($"Unhandled ABAP Exception: {bapiName}", apAbapRuntimeException);
                            Log.Error("Row No:" + counter + "  " + apAbapRuntimeException);
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"Couldn't Execute BAPI: {bapiName}", exception);
                        Log.Error("Row No:" + counter + "  " + exception);
                    }

                    if (responded) continue;
                    // Read the import structure RETURN to provide possible Messages
                    RFCStructure BapiReturn = bapiCall.Imports["RETURN"].ToStructure();
                    var message = BapiReturn["MESSAGE"].ToString();
                    Log.Information("=============================================Processing=======================================");
                    if (validateOnly)
                    {
                        if (message != "")
                        {
                            DateTime start = DateTime.Now;
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.NotAcceptable;
                            currentRecord.ResponseMessage.responseMessages = message;
                            DateTime end = DateTime.Now;
                            TimeSpan ts = end - start;
                            Log.Information("Validation not success for row no " + counter + "and time taken in seconds : " + ts.TotalSeconds);

                            Log.Information("Validation not success at Row No:" + counter + "  " + message);
                        }
                        else
                        {
                            DateTime start = DateTime.Now;
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.OK;
                            currentRecord.ResponseMessage.responseMessages = "Validation Success";
                            DateTime end = DateTime.Now;
                            TimeSpan ts = end - start;
                            Log.Information("validation for row no " + counter + "and time taken in seconds : " + ts.TotalSeconds);

                            Log.Information("Validation Success at Row No:" + counter);
                        }

                    }
                    else
                    {
                        RFCStructure BapiDocRef = bapiCall.Imports["DOCUMENTREFERENCE"].ToStructure();
                        var objectType = BapiDocRef["OBJ_TYPE"].ToString();
                        var objectKey = BapiDocRef["OBJ_KEY"].ToString();
                        RFCStructure Bapiposa = bapiCall.Imports["EV_EXTENSION"].ToStructure();
                        var posaloas = Bapiposa["POSA_LOSA"].ToString();
                        var docNo = Bapiposa["DOC_NUMBER"].ToString();

                        if (objectKey != "" &&
                            (BapiReturn["TYPE"].ToString() == "S" ||
                             BapiReturn["TYPE"].ToString() == "I"))
                        {
                            // Commit BAPI
                            DateTime start = DateTime.Now;
                            bool commitResult = CommitBapi(r3Connection, "X");
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.OK;
                            currentRecord.ResponseMessage.responsePosaLosa = posaloas;
                            currentRecord.ResponseMessage.responseMessages =
                                "Disposal Successful - Message: " + message +
                                 " - Document No: " + docNo + "  for Company Code:" + currentRecord.CompanyCode.PadLeft(4, '0') +
                                " - Document Details: " + objectKey;
                            DateTime end = DateTime.Now;
                            TimeSpan ts = end - start;
                            Log.Information("Committed record : row no" + counter + "and time taken in seconds : " + ts.TotalSeconds);
                            Log.Information("Disposal Successful at Row No:" + counter + "  " + message);
                        }
                        else
                        {
                            // Rollback BAPI
                            DateTime start = DateTime.Now;
                            bool rollBackResult = RollBackBapi(r3Connection);
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.NotAcceptable;
                            currentRecord.ResponseMessage.responseMessages =
                                "Disposal Unsuccessful - Message: " + message;
                            DateTime end = DateTime.Now;
                            TimeSpan ts = end - start;
                            Log.Information("Rollback record : row no" + counter + "and time taken is seconds : " + ts.TotalSeconds);

                            Log.Information("Disposal Unsuccessful at Row No:" + counter + "  " + message);
                        }
                    }
                    Log.Information("======================================================================================================");
                    counter++;
                }

                assetDisposalRequest.ResponseMessage.responseCode = HttpStatusCode.OK;
                assetDisposalRequest.ResponseMessage.responseMessages = "";
            }
            catch (Exception exception)
            {

                var message = "Error executing SAP Disposal BAPI";
                Log.Error(message + " at Row No: " + counter + "  " + exception);
                assetDisposalRequest.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                assetDisposalRequest.ResponseMessage.responseMessages = message;
            }
        }
        public void SapAssetTransfer(
        R3Connection r3Connection,
        AssetTransferRequest assetTransferRequest,
        bool validateOnly)
        {
            assetTransferRequest.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                // Create a Bapi object, fill parameters and execute

                var bapiName = "BAPI_FIXEDASSET_CHANGE";
                int counter = 1;
                RFCFunction bapiCall = null;
                Log.Information("Calling BAPI: " + bapiName);
                foreach (var currentRecord in assetTransferRequest.TransferRecords)
                {
                    bool responded = false;

                    if (bapiCall != null)
                    {
                        bapiCall = null;
                    }

                    bapiCall = r3Connection.CreateFunction(bapiName);
                    RFCStructure timeDependentData = bapiCall.Exports["TIMEDEPENDENTDATA"].ToStructure();
                    RFCStructure timeDependentDataX = bapiCall.Exports["TIMEDEPENDENTDATAX"].ToStructure();
                    String companyCode = Convert.ToString(currentRecord.CompanyCode);
                    String assetNumber = Convert.ToString(currentRecord.AssetNumber);
                    String subNumber = Convert.ToString(currentRecord.SubNumber);
                    String newCostCentre = Convert.ToString(currentRecord.NewCostCentre);
                    String newInternalOrder = Convert.ToString(currentRecord.NewInternalOrder);
                    String newLocation = Convert.ToString(currentRecord.NewLocation);                  
                    if (companyCode != null || companyCode != "")
                    {
                        bapiCall.Exports["COMPANYCODE"].ParamValue = Convert.ToString(currentRecord.CompanyCode);
                    }
                    else
                    {
                        bapiCall.Exports["COMPANYCODE"].ParamValue = " ";
                    }
                    if (assetNumber != null || assetNumber != "")
                    {
                        bapiCall.Exports["ASSET"].ParamValue = Convert.ToString(currentRecord.AssetNumber).PadLeft(12, '0');
                    }
                    else
                    {
                        bapiCall.Exports["ASSET"].ParamValue = " ";
                    }
                    if (subNumber != null || subNumber != "")
                    {
                        bapiCall.Exports["SUBNUMBER"].ParamValue = Convert.ToString(currentRecord.SubNumber).PadLeft(4, '0');
                    }
                    else
                    {
                        bapiCall.Exports["SUBNUMBER"].ParamValue = " ";
                    }
                    if (newCostCentre == null || newCostCentre == "")
                    {
                        timeDependentData["COSTCENTER"] = " ";
                        timeDependentDataX["COSTCENTER"] = " ";

                    }
                    else
                    {

                        timeDependentDataX["COSTCENTER"] = "X";
                        timeDependentDataX["INTERN_ORD"] = "X";
                        timeDependentData["COSTCENTER"] = Convert.ToString(currentRecord.NewCostCentre).PadLeft(10, '0');

                    }

                    if (newInternalOrder == null || newInternalOrder == "")
                    {
                        timeDependentData["INTERN_ORD"] = " ";
                        timeDependentDataX["INTERN_ORD"] = " ";

                    }
                    else

                    {
                        bool isLetter = !String.IsNullOrEmpty(newInternalOrder) && Char.IsLetter(newInternalOrder[0]);
                        if (isLetter == false)
                        {
                            timeDependentDataX["INTERN_ORD"] = "X";
                            timeDependentDataX["COSTCENTER"] = "X";
                            timeDependentData["INTERN_ORD"] = Convert.ToString(currentRecord.NewInternalOrder).PadLeft(12, '0');
                        }
                        else
                        {
                            timeDependentDataX["INTERN_ORD"] = "X";
                            timeDependentDataX["COSTCENTER"] = "X";
                            timeDependentData["INTERN_ORD"] = newInternalOrder;
                        }
                    }


                    if (newLocation == null || newLocation == "")
                    {

                        timeDependentData["LOCATION"] = " ";
                        timeDependentDataX["LOCATION"] = " ";
                    }
                    else
                    {
                        timeDependentDataX["LOCATION"] = "X";
                        timeDependentData["LOCATION"] = Convert.ToString(currentRecord.NewLocation);
                    }
                    
                    try
                    {
                        
                        bapiCall.Execute();
                    }

                    catch (ABAPRuntimeException apAbapRuntimeException)
                    {
                        if (apAbapRuntimeException.ABAPException == "RFC_ERROR_SYSTEM_FAILURE")
                        {
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
                            currentRecord.ResponseMessage.responseMessages = apAbapRuntimeException.Description;
                            responded = true;
                            Log.Error("Row No:" + counter + "  " + currentRecord.ResponseMessage.responseMessages);
                        }
                        else
                        {
                            throw new Exception($"Unhandled ABAP Exception: {bapiName}", apAbapRuntimeException);
                          
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"Couldn't Execute BAPI: {bapiName}", exception);
                        
                    }

                    if (responded) continue;
                    // Read the import structure RETURN to provide possible Messages                   
                    RFCStructure BapiReturn = bapiCall.Imports["RETURN"].ToStructure();
                    
                    var message = BapiReturn["MESSAGE"].ToString();
                    if (validateOnly)
                    {
                        if (message != "")
                        {
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.NotAcceptable;
                            currentRecord.ResponseMessage.responseMessages = message;
                            Log.Information("Validation not success at Row No:" + counter + "  " + message);
                        }
                        else
                        {
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.OK;
                            currentRecord.ResponseMessage.responseMessages = "Validation Success";
                            Log.Information("Validation Success at Row No:" + counter);
                        }

                    }
                    else
                    {
                        try
                        {
                            var type = BapiReturn["TYPE"].ToString();

                            if (
                                (BapiReturn["TYPE"].ToString() == "S" ||
                                 BapiReturn["TYPE"].ToString() == "I"))
                            {
                                //Commit BAPI
                                bool commitResult = CommitBapi(r3Connection, " ");

                                currentRecord.ResponseMessage.responseCode = HttpStatusCode.OK;

                                currentRecord.ResponseMessage.responseMessages =
                                    "Asset Transfer Successful" + message;

                                Log.Information("Asset Transfer Successful at Row No:" + counter + "  " + message);
                            }
                            else
                            {
                                //RollBack BAPI
                                bool rollBackResult = RollBackBapi(r3Connection);
                                currentRecord.ResponseMessage.responseCode = HttpStatusCode.NotAcceptable;
                                currentRecord.ResponseMessage.responseMessages =
                                    "Asset Transfer Unsuccessful" + message;
                                Log.Information("Asset Transfer Unsuccessful at Row No:" + counter + "  " + message);
                                continue;

                            }
                        }
                        
                        catch (Exception exception)
                        {
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
                            currentRecord.ResponseMessage.responseMessages = exception.ToString();
                            continue;
                        }
                    }
                }
                
                counter++;
                assetTransferRequest.ResponseMessage.responseCode = HttpStatusCode.OK;
                assetTransferRequest.ResponseMessage.responseMessages = "";
            }
            catch (Exception exception)
            {

                var message = "Error connecting to SAP";
                Log.Error(exception, message);              
                assetTransferRequest.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                assetTransferRequest.ResponseMessage.responseMessages = message;
            }
        }
        public void SapAssetTransferSearch(
         R3Connection r3Connection,
         AssetTransferSearch assetTransferSearch)
        {
            assetTransferSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);
            int counter = 0;
            try
            {
                // Create a Bapi object, fill parameters and execute
                var bapiCall = r3Connection.CreateFunction("ZBAPI_FIXED_ASSET_DETAILS");
                var bapiName = "ZBAPI_FIXED_ASSET_DETAILS";
                Log.Information("Calling BAPI: " + bapiName);
                bapiCall.Exports["IV_COMPANYCODE"].ParamValue = assetTransferSearch.CompanyCode == null
                   ? ""
                   : assetTransferSearch.CompanyCode.PadLeft(4, '0');
                bapiCall.Exports["IV_MAINASSET"].ParamValue =
                    assetTransferSearch.Asset == null ? "" : assetTransferSearch.Asset.PadLeft(12, '0');
                bapiCall.Exports["IV_ASSETCLASS"].ParamValue = assetTransferSearch.AssetClass == null
                    ? ""
                    : assetTransferSearch.AssetClass.PadLeft(8, '0');
                bapiCall.Exports["IV_ASSETDESCRPT"].ParamValue = assetTransferSearch.AssetDescription ?? "";
                bapiCall.Exports["IV_COSTCENTRE"].ParamValue = assetTransferSearch.CostCentre == null
                    ? ""
                    : assetTransferSearch.CostCentre.PadLeft(10, '0');
              //  bapiCall.Exports["IV_LIMIT"].ParamValue = assetTransferSearch.SearchResultLimit.ToString();
                if(Convert.ToInt32(assetTransferSearch.SearchResultLimit) > 5000)
                {
                    assetTransferSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                    assetTransferSearch.ResponseMessage.responseMessages = "Please Enter Limit Less Than 5000";
                }
                else
                {
                    bapiCall.Exports["IV_LIMIT"].ParamValue = assetTransferSearch.SearchResultLimit.ToString();
                    bapiCall.Execute();

                    // Read the response table

                    var resultTable = bapiCall.Tables["ET_DETAILS"].ToADOTable();
                    if (resultTable.Rows.Count == 0)
                    {
                        assetTransferSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                        assetTransferSearch.ResponseMessage.responseMessages = "No Records Found";

                    }
                    else
                    {
                        assetTransferSearch.SearchResults = new List<AssetTransferSearchResult>();

                        foreach (DataRow currentRow in resultTable.Rows)
                        {
                            if (assetTransferSearch.SearchResultLimit != null &&
                                counter == assetTransferSearch.SearchResultLimit)
                            {
                                break;
                            }



                            decimal accountingBookCost;
                            decimal accountingBookWrittenDown;

                            decimal.TryParse(currentRow["ACCBKCOST"].ToString(), out accountingBookCost);
                            decimal.TryParse(currentRow["ACCBKWDV"].ToString(), out accountingBookWrittenDown);

                            assetTransferSearch
                                .SearchResults
                                .Add(
                                    new AssetTransferSearchResult(
                                        currentRow["COMPANY_CODE"].ToString(),
                                        currentRow["ASSET_NO"].ToString(),
                                        currentRow["SUB_ASSET_NO"].ToString(),
                                        currentRow["ASSET_CLASS"].ToString(),
                                        currentRow["ASSET_DESC"].ToString(),
                                        currentRow["SAP_EQUIP_NO"].ToString(),
                                        currentRow["COST_CENTRE"].ToString(),
                                        currentRow["INTERNAL_ORDER"].ToString(),
                                        currentRow["LOCATION"].ToString(),
                                        currentRow["FUNC_LOC"].ToString(),
                                        accountingBookCost,
                                        accountingBookWrittenDown
                                    )
                                );

                            counter++;
                            Log.Information("Added Asset Transfer LookUp Data to List Successfully ");
                            assetTransferSearch.ResponseMessage.responseCode = HttpStatusCode.OK;
                        }
                    }
                }                
            }
            catch (Exception exception)
            {

                var message = "Error connecting to SAP";
                Log.Error(exception, message);

                assetTransferSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                assetTransferSearch.ResponseMessage.responseMessages = message;
            }
        }

        public void SapAssetAcquisitionSearch(
        R3Connection r3Connection,
        AssetAcquisitionSearch assetAcquisitionSearch)
        {
            assetAcquisitionSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);
            int counter = 0;
            try
            {
                // Create a Bapi object, fill parameters and execute
                var bapiCall = r3Connection.CreateFunction("ZBAPI_FIXED_ASSET_DETAILS");
                var bapiName = "ZBAPI_FIXED_ASSET_DETAILS";
                Log.Information("Calling BAPI: " + bapiName);
                bapiCall.Exports["IV_COMPANYCODE"].ParamValue = assetAcquisitionSearch.CompanyCode == null
                   ? ""
                   : assetAcquisitionSearch.CompanyCode.PadLeft(4, '0');
                bapiCall.Exports["IV_MAINASSET"].ParamValue =
                    assetAcquisitionSearch.Asset == null ? "" : assetAcquisitionSearch.Asset.PadLeft(12, '0');
                bapiCall.Exports["IV_ASSETCLASS"].ParamValue = assetAcquisitionSearch.AssetClass == null
                    ? ""
                    : assetAcquisitionSearch.AssetClass.PadLeft(8, '0');
                bapiCall.Exports["IV_ASSETDESCRPT"].ParamValue = assetAcquisitionSearch.AssetDescription ?? "";
                bapiCall.Exports["IV_COSTCENTRE"].ParamValue = assetAcquisitionSearch.CostCentre == null
                    ? ""
                    : assetAcquisitionSearch.CostCentre.PadLeft(10, '0');
                //  bapiCall.Exports["IV_LIMIT"].ParamValue = assetTransferSearch.SearchResultLimit.ToString();
                if (Convert.ToInt32(assetAcquisitionSearch.SearchResultLimit) > 5000)
                {
                    assetAcquisitionSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                    assetAcquisitionSearch.ResponseMessage.responseMessages = "Please Enter Limit Less Than 5000";
                }
                else
                {
                    bapiCall.Exports["IV_LIMIT"].ParamValue = assetAcquisitionSearch.SearchResultLimit.ToString();
                    bapiCall.Execute();

                    // Read the response table

                    var resultTable = bapiCall.Tables["ET_DETAILS"].ToADOTable();
                    if (resultTable.Rows.Count == 0)
                    {
                        assetAcquisitionSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                        assetAcquisitionSearch.ResponseMessage.responseMessages = "No Records Found";

                    }
                    else
                    {
                        assetAcquisitionSearch.SearchResults = new List<AssetAcquisitionSearchResult>();

                        foreach (DataRow currentRow in resultTable.Rows)
                        {
                            if (assetAcquisitionSearch.SearchResultLimit != null &&
                                counter == assetAcquisitionSearch.SearchResultLimit)
                            {
                                break;
                            }



                            decimal accountingBookCost;
                            decimal accountingBookWrittenDown;

                            decimal.TryParse(currentRow["ACCBKCOST"].ToString(), out accountingBookCost);
                            decimal.TryParse(currentRow["ACCBKWDV"].ToString(), out accountingBookWrittenDown);

                            assetAcquisitionSearch
                                .SearchResults
                                .Add(
                                    new AssetAcquisitionSearchResult(
                                        currentRow["COMPANY_CODE"].ToString(),
                                        currentRow["ASSET_NO"].ToString(),
                                        currentRow["SUB_ASSET_NO"].ToString(),
                                        currentRow["ASSET_CLASS"].ToString(),
                                        currentRow["ASSET_DESC"].ToString(),
                                        currentRow["SAP_EQUIP_NO"].ToString(),
                                        currentRow["COST_CENTRE"].ToString(),
                                        currentRow["INTERNAL_ORDER"].ToString(),
                                        currentRow["LOCATION"].ToString(),
                                        currentRow["FUNC_LOC"].ToString(),
                                        accountingBookCost,
                                        accountingBookWrittenDown
                                    )
                                );

                            counter++;
                            Log.Information("Added Asset Transfer LookUp Data to List Successfully ");
                            assetAcquisitionSearch.ResponseMessage.responseCode = HttpStatusCode.OK;
                        }
                    }
                }
            }
            catch (Exception exception)
            {

                var message = "Error connecting to SAP";
                Log.Error(exception, message);

                assetAcquisitionSearch.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                assetAcquisitionSearch.ResponseMessage.responseMessages = message;
            }
        }

        public void SapAssetAcquisition(
       R3Connection r3Connection,
       AssetAcquisitionRequest assetAcquisitionRequest,       
       bool validateOnly
            ,string asserttype
            )
        {
            assetAcquisitionRequest.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                var bapiName="";
                // Create a Bapi object, fill parameters and execute
                if (asserttype == "host")
                {
                    bapiName = "ZBAPI_FIXEDASSET_CREATEHOST";
                }
                else
                {
                    bapiName = "ZBAPI_FIXEDASSET_CREATESUBHOST";
                }
                int counter = 1;
                RFCFunction bapiCall = null;
                Log.Information("Calling BAPI: " + bapiName);
                foreach (var currentRecord in assetAcquisitionRequest.AcquisitionRecords)
                {
                    bool responded = false;

                    if (bapiCall != null)
                    {
                        bapiCall = null;
                    }

                    bapiCall = r3Connection.CreateFunction(bapiName);
                    RFCStructure rfData = bapiCall.Exports["RFCDATA"].ToStructure();

                    rfData["ASSETCLASS"] = currentRecord.Class == null
                     ? " "
                     : currentRecord.Class;
                    rfData["COMPANYCODE"] = currentRecord.CompanyCode == null
                    ? " "
                    : currentRecord.CompanyCode;
                    rfData["DESCRIPT"] = currentRecord.AssetDesc == null
                    ? " "
                    : currentRecord.AssetDesc;                    
                   
                    rfData["MANUFACTURER"] = currentRecord.Manufacturer == null
                    ? " "
                    : currentRecord.Manufacturer;
                   
                    rfData["SERIAL_NO"] = currentRecord.SerialNo == null
                    ? " "
                    : currentRecord.SerialNo;

                    rfData["INITIAL_ACQ"] = currentRecord.AcquisitionDate == null
                   ? " "
                   : currentRecord.AcquisitionDate;

                    rfData["COSTCENTER"] = currentRecord.CostCenter == null
                   ? " "
                   : currentRecord.CostCenter;
                    rfData["INTERN_ORD"] = currentRecord.InternalOrder == null
                   ? " "
                   : currentRecord.InternalOrder;
                    rfData["LOCATION"] = currentRecord.Location == null
                 ? " "
                 : currentRecord.Location;
                    rfData["QUANTITY"] = currentRecord.Quantity == null
                 ? " "
                 : currentRecord.Quantity;
                  
                    rfData["DESCRIPT2"] = currentRecord.AdditionalDesc2 == null
                 ? " "
                 : currentRecord.AdditionalDesc2;
                    try
                    {

                        bapiCall.Execute();
                    }

                    catch (ABAPRuntimeException apAbapRuntimeException)
                    {
                        if (apAbapRuntimeException.ABAPException == "RFC_ERROR_SYSTEM_FAILURE")
                        {
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
                            currentRecord.ResponseMessage.responseMessages = apAbapRuntimeException.Description;
                            responded = true;
                            Log.Error("Row No:" + counter + "  " + currentRecord.ResponseMessage.responseMessages);
                        }
                        else
                        {
                            throw new Exception($"Unhandled ABAP Exception: {bapiName}", apAbapRuntimeException);

                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"Couldn't Execute BAPI: {bapiName}", exception);

                    }

                    if (responded) continue;
                    // Read the import structure RETURN to provide possible Messages                   
                    RFCStructure BapiReturn = bapiCall.Imports["RETURN"].ToStructure();

                    var message = BapiReturn["MESSAGE"].ToString();
                    if (validateOnly)
                    {
                        if (message != "")
                        {
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.NotAcceptable;
                            currentRecord.ResponseMessage.responseMessages = message;
                            Log.Information("Validation not success at Row No:" + counter + "  " + message);
                        }
                        else
                        {
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.OK;
                            currentRecord.ResponseMessage.responseMessages = "Validation Success";
                            Log.Information("Validation Success at Row No:" + counter);
                        }

                    }
                    else
                    {

                        var type = BapiReturn["TYPE"].ToString();


                        if (
                            (BapiReturn["TYPE"].ToString() == "S" ||
                             BapiReturn["TYPE"].ToString() == "I"))
                        {
                            //Commit BAPI
                            bool commitResult = CommitBapi(r3Connection, " ");

                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.OK;

                            currentRecord.ResponseMessage.responseMessages =
                                "Asset Acquistion Successful at Row No:" + counter + "  " + message;

                            Log.Information("Asset Acquistion Successful at Row No:" + counter + "  " + message);
                        }
                        else
                        {
                            //RollBack BAPI
                            bool rollBackResult = RollBackBapi(r3Connection);
                            currentRecord.ResponseMessage.responseCode = HttpStatusCode.NotAcceptable;
                            currentRecord.ResponseMessage.responseMessages =
                                "Asset Acquistion Unsuccessful - Message: " + message;
                            Log.Information("Asset Acquistion Unsuccessful at Row No:" + counter + "  " + message);
                            continue;

                        }
                    }
                }

                counter++;
                assetAcquisitionRequest.ResponseMessage.responseCode = HttpStatusCode.OK;
                assetAcquisitionRequest.ResponseMessage.responseMessages = "";
            }
            catch (Exception exception)
            {

                var message = "Error connecting to SAP";
                Log.Error(exception, message);
                assetAcquisitionRequest.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
                assetAcquisitionRequest.ResponseMessage.responseMessages = message;
            }
        }

      //  public void SapAssetCreationSub(
      //R3Connection r3Connection,
      //AssetCreationSubRequest assetCreationSubRequest,
      //bool validateOnly)
      //  {
      //      assetCreationSubRequest.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

      //      try
      //      {
      //          // Create a Bapi object, fill parameters and execute

      //          var bapiName = "BAPI_FIXEDASSET_CHANGE";
      //          int counter = 1;
      //          RFCFunction bapiCall = null;
      //          Log.Information("Calling BAPI: " + bapiName);
      //          foreach (var currentRecord in assetCreationSubRequest.CreationSubRecords)
      //          {
      //              bool responded = false;

      //              if (bapiCall != null)
      //              {
      //                  bapiCall = null;
      //              }

                   

      //                  bapiCall = r3Connection.CreateFunction(bapiName);
      //                  RFCStructure rfData = bapiCall.Exports["RFCDATA"].ToStructure();

      //                  rfData["ASSETCLASS"] = currentRecord.AssetClass == null
      //                   ? " "
      //                   : currentRecord.AssetClass;
      //                  rfData["COMPANYCODE"] = currentRecord.CompanyCode == null
      //                  ? " "
      //                  : currentRecord.CompanyCode;
      //                  rfData["DESCRIPT"] = currentRecord.Description == null
      //                  ? " "
      //                  : currentRecord.Description;
      //                  rfData["PLATE_NO"] = currentRecord.PlateNo == null
      //                  ? " "
      //                  : currentRecord.PlateNo;
      //                  rfData["INVENT_NO"] = currentRecord.InventNo == null
      //                  ? " "
      //                  : currentRecord.InventNo;
      //                  rfData["MANUFACTURER"] = currentRecord.Manufacturer == null
      //                  ? " "
      //                  : currentRecord.Manufacturer;
      //                  rfData["NOTE"] = currentRecord.Note == null
      //                  ? " "
      //                  : currentRecord.Note;
      //                  rfData["SERIAL_NO"] = currentRecord.SerialNo == null
      //                  ? " "
      //                  : currentRecord.SerialNo;

      //                  rfData["INITIAL_ACQ"] = currentRecord.InitialAcq == null
      //                 ? " "
      //                 : currentRecord.InitialAcq;

      //                  rfData["COSTCENTER"] = currentRecord.CostCenter == null
      //                 ? " "
      //                 : currentRecord.CostCenter;
      //                  rfData["INTERN_ORD"] = currentRecord.InternalOrd == null
      //                 ? " "
      //                 : currentRecord.InternalOrd;
      //                  rfData["LOCATION"] = currentRecord.Location == null
      //               ? " "
      //               : currentRecord.Location;
      //                  rfData["QUANTITY"] = currentRecord.Quantity == null
      //               ? " "
      //               : currentRecord.Quantity;
      //                  rfData["BASE_UOM"] = currentRecord.BaseOum == null
      //               ? " "
      //               : currentRecord.BaseOum;
      //                  rfData["DESCRIPT2"] = currentRecord.Description2 == null
      //               ? " "
      //               : currentRecord.Description2;
      //                  rfData["ROOM"] = currentRecord.Room == null
      //               ? " "
      //               : currentRecord.Room;

      //                  try
      //                  {

      //                  bapiCall.Execute();
      //              }

      //              catch (ABAPRuntimeException apAbapRuntimeException)
      //              {
      //                  if (apAbapRuntimeException.ABAPException == "RFC_ERROR_SYSTEM_FAILURE")
      //                  {
      //                      currentRecord.ResponseMessage.responseCode = HttpStatusCode.BadRequest;
      //                      currentRecord.ResponseMessage.responseMessages = apAbapRuntimeException.Description;
      //                      responded = true;
      //                      Log.Error("Row No:" + counter + "  " + currentRecord.ResponseMessage.responseMessages);
      //                  }
      //                  else
      //                  {
      //                      throw new Exception($"Unhandled ABAP Exception: {bapiName}", apAbapRuntimeException);

      //                  }
      //              }
      //              catch (Exception exception)
      //              {
      //                  throw new Exception($"Couldn't Execute BAPI: {bapiName}", exception);

      //              }

      //              if (responded) continue;
      //              // Read the import structure RETURN to provide possible Messages                   
      //              RFCStructure BapiReturn = bapiCall.Imports["RETURN"].ToStructure();

      //              var message = BapiReturn["MESSAGE"].ToString();
      //              if (validateOnly)
      //              {
      //                  if (message != "")
      //                  {
      //                      currentRecord.ResponseMessage.responseCode = HttpStatusCode.NotAcceptable;
      //                      currentRecord.ResponseMessage.responseMessages = message;
      //                      Log.Information("Validation not success at Row No:" + counter + "  " + message);
      //                  }
      //                  else
      //                  {
      //                      currentRecord.ResponseMessage.responseCode = HttpStatusCode.OK;
      //                      currentRecord.ResponseMessage.responseMessages = "Validation Success";
      //                      Log.Information("Validation Success at Row No:" + counter);
      //                  }

      //              }
      //              else
      //              {

      //                  var type = BapiReturn["TYPE"].ToString();


      //                  if (
      //                      (BapiReturn["TYPE"].ToString() == "S" ||
      //                       BapiReturn["TYPE"].ToString() == "I"))
      //                  {
      //                      //Commit BAPI
      //                      bool commitResult = CommitBapi(r3Connection, " ");

      //                      currentRecord.ResponseMessage.responseCode = HttpStatusCode.OK;

      //                      currentRecord.ResponseMessage.responseMessages =
      //                          "Asset Transfer Successful at Row No:" + counter + "  " + message;

      //                      Log.Information("Asset Transfer Successful at Row No:" + counter + "  " + message);
      //                  }
      //                  else
      //                  {
      //                      //RollBack BAPI
      //                      bool rollBackResult = RollBackBapi(r3Connection);
      //                      currentRecord.ResponseMessage.responseCode = HttpStatusCode.NotAcceptable;
      //                      currentRecord.ResponseMessage.responseMessages =
      //                          "Asset Transfer Unsuccessful - Message: " + message;
      //                      Log.Information("Asset Transfer Unsuccessful at Row No:" + counter + "  " + message);
      //                      continue;

      //                  }
      //              }
      //          }

      //          counter++;
      //          assetCreationSubRequest.ResponseMessage.responseCode = HttpStatusCode.OK;
      //          assetCreationSubRequest.ResponseMessage.responseMessages = "";
      //      }
      //      catch (Exception exception)
      //      {

      //          var message = "Error connecting to SAP";
      //          Log.Error(exception, message);
      //          assetCreationSubRequest.ResponseMessage.responseCode = HttpStatusCode.InternalServerError;
      //          assetCreationSubRequest.ResponseMessage.responseMessages = message;
      //      }
      //  }

        private bool CommitBapi(
            R3Connection r3Connection,
            string waitFlag)
        {
            var bapiName = "BAPI_TRANSACTION_COMMIT";
            var response = false;
            try
            {
                // Create a Bapi object, fill parameters and execute

                var bapiCall = r3Connection.CreateFunction(bapiName);

                var commitParamStructure = bapiCall.Exports["WAIT"];
                commitParamStructure.ParamValue = waitFlag;
                bapiCall.Execute();

                // Read the import structure RETURN to provide possible Messages
                RFCStructure BapiReturn = bapiCall.Imports["RETURN"].ToStructure();
                var message = BapiReturn["MESSAGE"].ToString();

                if (message == "")
                {
                    response = true;
                }
                else
                {
                    Log.Error($"BAPI: {bapiName} - returned error: {message}");
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"BAPI {bapiName} failed");
            }
            return response;
        }

        private bool RollBackBapi(
            R3Connection r3Connection)
        {
            var bapiName = "BAPI_TRANSACTION_ROLLBACK";
            var response = false;
            try
            {
                // Create a Bapi object, fill parameters and execute

                var bapiCall = r3Connection.CreateFunction(bapiName);

                bapiCall.Execute();

                // Read the import structure RETURN to provide possible Messages
                RFCStructure BapiReturn = bapiCall.Imports["RETURN"].ToStructure();
                var message = BapiReturn["MESSAGE"].ToString();

                if (message == "")
                {
                    response = true;
                }
                else
                {
                    Log.Error($"BAPI: {bapiName} - returned error: {message}");
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"BAPI {bapiName} failed");
            }
            return response;
        }
    }
}