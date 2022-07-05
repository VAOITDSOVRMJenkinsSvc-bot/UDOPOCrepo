using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.IO;
using Microsoft.Xrm.Sdk.Query;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Diagnostics;
using VRM.IntegrationServicebus.AddDependent.CrmModel;
using CheckBox = DocumentFormat.OpenXml.Wordprocessing.CheckBox;
using MCSUtilities2011;

namespace UDO.LOB.DependentMaintenance
{
    public class DocGen
    {

        private static string font;
        private static string fontsize;
        #region Document Entity Gets
        /// <summary>
        /// Get the DocGen Document
        /// </summary>
        /// <param name="service"></param>
        /// <param name="documentName"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static crme_docgendocument GetDocGenDocument(IOrganizationService service, string documentName, IMCSLogger logger)
        {
            logger.setMethod = "GetDocGenDocument";
            logger.WriteGranularTimingMessage("start GetDocGenDocument");

            crme_docgendocument dgd = null;
            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                dgd = (from doc in serviceContext.crme_docgendocumentSet
                       where doc.crme_name == documentName
                       select doc).FirstOrDefault();

            }
            logger.WriteGranularTimingMessage("end GetWordProcessingDocument");

            return dgd;
        }
        /// <summary>
        /// Return Document Bookmarks and mappins for a specific document.
        /// </summary>
        /// <param name="parentGuid"></param>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<crme_docgendocumentfield> GetDocGenDocumentFields(Guid parentGuid, IOrganizationService service, IMCSLogger logger)
        {
            logger.setMethod = "GetDocGenDocumentFields";
            logger.WriteGranularTimingMessage("start GetDocGenDocumentFields");

            List<crme_docgendocumentfield> fields = null;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                fields = serviceContext.crme_docgendocumentfieldSet.Where(d => d.crme_DocumentId.Id.Equals(parentGuid)).ToList();

            }

            if (fields.Count == 0)
            {
                logger.WriteGranularTimingMessage("GetDocGenDocumentFields did not return results.");

            }

            logger.WriteGranularTimingMessage("end GetDocGenDocumentFields");

            return fields;

        }
        /// <summary>
        /// REturn Document Field Formats
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static List<crme_docgenfieldformat> GetFieldFormat(IOrganizationService service, IMCSLogger logger)
        {
            logger.setMethod = "GetFieldFormat";
            logger.WriteGranularTimingMessage("start GetFieldFormat");

            List<crme_docgenfieldformat> fieldFormats;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {

                fieldFormats = serviceContext.crme_docgenfieldformatSet.ToList();
            }

            logger.WriteGranularTimingMessage("end GetFieldFormat");

            return fieldFormats;
        }
        public static List<crme_docgentablemapping> GetDocGenDynamicTableMappings(Guid parentGuid, IOrganizationService service, IMCSLogger logger)
        {
            logger.setMethod = "GetDocGenDynamicTableMappings";
            logger.WriteGranularTimingMessage("start GetDocGenDynamicTableMappings");

            List<crme_docgentablemapping> tableMappings;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {

                tableMappings =
                    serviceContext.crme_docgentablemappingSet.Where(
                        t => t.crme_DocumentId.Id.Equals(parentGuid) && t.crme_Type.Value.Equals(935950001)).ToList();
            }

            logger.WriteGranularTimingMessage("end GetDocGenDynamicTableMappings");
            return tableMappings;
        }
        public static List<crme_docgentablemapping> GetDocGenStaticTableMappings(Guid parentGuid, IOrganizationService service, IMCSLogger logger)
        {
            logger.setMethod = "GetDocGenStaticTableMappings";
            logger.WriteGranularTimingMessage("start GetDocGenStaticTableMappings");

            List<crme_docgentablemapping> tableMappings;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {

                tableMappings =
                    serviceContext.crme_docgentablemappingSet.Where(
                        t => t.crme_DocumentId.Id.Equals(parentGuid) && t.crme_Type.Value.Equals(935950000)).ToList();
            }

            logger.WriteGranularTimingMessage("end GetDocGenStaticTableMappings");
            return tableMappings;
        }
        public static List<crme_docgentablefields> GetDocGenTableFields(Guid parentGuid, IOrganizationService service, IMCSLogger logger)
        {

            logger.setMethod = "GetDocGenTableFields";
            logger.WriteGranularTimingMessage("start GetDocGenTableFields");
            List<crme_docgentablefields> tableFields;
            using (VRMXRM serviceContext = new VRMXRM(service))
            {

                tableFields = serviceContext.crme_docgentablefieldsSet.Where(f => f.crme_DocGenTableMappingId.Id.Equals(parentGuid)).ToList();
            }

            logger.WriteGranularTimingMessage("end GetDocGenTableFields");
            return tableFields;
        }
        #endregion
        public static MemoryStream GetWordProcessingDocument(IOrganizationService service, string documentName, IMCSLogger logger)
        {
            logger.setMethod = "GetWordProcessingDocument";
            logger.WriteGranularTimingMessage("start GetWordProcessingDocument");

            MemoryStream ms = new MemoryStream();
            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                var document = (from doc in serviceContext.crme_docgendocumentSet
                                join a in serviceContext.AnnotationSet
                                on doc.Id equals a.ObjectId.Id
                                where doc.crme_name == documentName
                                select a.DocumentBody).FirstOrDefault();

                if (document != null)
                {
                    byte[] byteArray = Convert.FromBase64String(document);

                    ms.Write(byteArray, 0, byteArray.Length);
                }
                else
                {
                    throw new Exception("GetWordProcessingDocument: Document not Found.");
                }


            }
            logger.WriteGranularTimingMessage("end GetWordProcessingDocument");

            return ms;

        }
        public static Entity FetchPrimaryXml(IOrganizationService service, string xmlQuery, IMCSLogger logger)
        {
            logger.setMethod = "FetchPrimaryXml";
            logger.WriteGranularTimingMessage("start FetchPrimaryXml");

            Entity entity = null;
            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                EntityCollection entities = service.RetrieveMultiple(new FetchExpression(xmlQuery));
                if (entities != null && entities.Entities.Count > 0) entity = entities.Entities[0];
            }
            logger.WriteGranularTimingMessage("end FetchPrimaryXml");

            return entity;
        }
        public static EntityCollection FetchTableXml(IOrganizationService service, string xmlQuery, IMCSLogger logger)
        {
            logger.setMethod = "FetchTableXml";
            logger.WriteGranularTimingMessage("start FetchTableXml");

            EntityCollection entities = null;
            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                // Convert the FetchXML into a query expression.
                var conversionRequest = new FetchXmlToQueryExpressionRequest
                {
                    FetchXml = xmlQuery
                };

                var conversionResponse =
                    (FetchXmlToQueryExpressionResponse)service.Execute(conversionRequest);

                // Use the newly converted query expression to make a retrieve multiple
                // request to Microsoft Dynamics CRM.
                QueryExpression queryExpression = conversionResponse.Query;

                entities = service.RetrieveMultiple(queryExpression);

                //entities = service.RetrieveMultiple(new FetchExpression(xmlQuery));
            }
            logger.WriteGranularTimingMessage("end FetchTableXml");

            return entities;
        }
        public static byte[] CreateDocumentFromMaster(IOrganizationService service, Entity thisResource, string template, IMCSLogger logger)
        {
            logger.setMethod = "CreateDocumentFromMaster";
            logger.WriteGranularTimingMessage("start CreateDocumentFromMaster");


            byte[] bytes = new byte[0];

            byte[] byteArray = null;


            crme_docgenmasterdocument docMaster;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                docMaster = (from doc in serviceContext.crme_docgenmasterdocumentSet
                             where doc.crme_name == template
                             select doc).FirstOrDefault();
                var docs = from docToProcess in serviceContext.crme_docgendocumentSet
                           where docToProcess.crme_ParentDocumentId.Id.Equals(docMaster.crme_docgenmasterdocumentId.Value)
                           orderby docToProcess.crme_Sequence ascending
                           select new
                           {
                               docToProcess.crme_name,
                               docToProcess.crme_Sequence,
                               docToProcess.crme_DynamictablesOnly,
                               docToProcess.crme_DocGenQuery,
                               docToProcess.crme_Font,
                               docToProcess.crme_FontSize,
                               docToProcess.crme_ParentDocumentId
                           };
                var idx = 0;
                var totalDocs = 0;
                foreach (var thisDoc in docs)
                {
                    totalDocs += 1;

                }

                MemoryStream[] docStreams = new MemoryStream[totalDocs];
                foreach (var thisDoc in docs)
                {
                    string altChunkId = "AltChunkId" + idx;
                    //byte[] tempBytes = DocGen.CreateDocument(service, thisResource, thisDoc.crme_name, logger, altChunkId, idx);
                    docStreams[idx] = DocGen.RCCreateDocument(service, thisResource, thisDoc.crme_name, logger, altChunkId, idx);
                    //if (idx > 0)
                    //{
                    //    if (tempBytes != null)
                    //    {
                    //        bytes = ByteConcat(bytes, tempBytes);
                    //    }
                    //}
                    //else
                    //{
                    //    bytes = tempBytes;
                    //}
                    idx += 1;
                }
                if (totalDocs > 1)
                {
                    idx = 0;
                    using (WordprocessingDocument myDoc = WordprocessingDocument.Open(docStreams[0], true))
                    {

                        string altChunkId = "AltChunkId" + idx;

                        MainDocumentPart mainPart = myDoc.MainDocumentPart;


                        foreach (MemoryStream thisStream in docStreams)
                        {
                            if (idx != 0)
                            {
                                if (thisStream != null)
                                {
                                    //using (FileStream fileStream = File.Open("TestInsertedContent.docx", FileMode.Open))
                                    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                                        AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                                    thisStream.Position = 0;
                                    chunk.FeedData(thisStream);
                                    AltChunk altChunk = new AltChunk();
                                    altChunk.Id = altChunkId;
                                    mainPart.Document
                                        .Body
                                        .InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());

                                }
                            }
                            idx += 1;
                            altChunkId = "AltChunkId" + idx;
                        }

                    }


                }
                int bLenght = (int)docStreams[0].Length;
                byteArray = new byte[bLenght];
                docStreams[0].Position = 0;
                docStreams[0].Read(byteArray, 0, bLenght);

                logger.WriteGranularTimingMessage("end CreateDocument");
            }

            return byteArray;
        }


        public static byte[] CreateDocument674FromMaster(IOrganizationService service, Guid depID, string template, IMCSLogger logger)
        {
            logger.setMethod = "CreateDocument674FromMaster";
            logger.WriteGranularTimingMessage("start CreateDocument674FromMaster");


            byte[] bytes = new byte[0];

            byte[] byteArray = null;


            crme_docgenmasterdocument docMaster;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                docMaster = (from doc in serviceContext.crme_docgenmasterdocumentSet
                             where doc.crme_name == template
                             select doc).FirstOrDefault();
                var docs = from docToProcess in serviceContext.crme_docgendocumentSet
                           where docToProcess.crme_ParentDocumentId.Id.Equals(docMaster.crme_docgenmasterdocumentId.Value)
                           orderby docToProcess.crme_Sequence ascending
                           select new
                           {
                               docToProcess.crme_name,
                               docToProcess.crme_Sequence,
                               docToProcess.crme_DynamictablesOnly,
                               docToProcess.crme_DocGenQuery,
                               docToProcess.crme_Font,
                               docToProcess.crme_FontSize,
                               docToProcess.crme_ParentDocumentId
                           };
                var idx = 0;
                var totalDocs = 0;
                foreach (var thisDoc in docs)
                {
                    totalDocs += 1;

                }

                MemoryStream[] docStreams = new MemoryStream[totalDocs];
                foreach (var thisDoc in docs)
                {
                    string altChunkId = "AltChunkId" + idx;
                    //byte[] tempBytes = DocGen.CreateDocument(service, thisResource, thisDoc.crme_name, logger, altChunkId, idx);
                    docStreams[idx] = DocGen.CreateDocument674(service, depID, thisDoc.crme_name, logger, altChunkId, idx);
                    //if (idx > 0)
                    //{
                    //    if (tempBytes != null)
                    //    {
                    //        bytes = ByteConcat(bytes, tempBytes);
                    //    }
                    //}
                    //else
                    //{
                    //    bytes = tempBytes;
                    //}
                    idx += 1;
                }
                if (totalDocs > 1)
                {
                    idx = 0;
                    using (WordprocessingDocument myDoc = WordprocessingDocument.Open(docStreams[0], true))
                    {

                        string altChunkId = "AltChunkId" + idx;

                        MainDocumentPart mainPart = myDoc.MainDocumentPart;


                        foreach (MemoryStream thisStream in docStreams)
                        {
                            if (idx != 0)
                            {
                                if (thisStream != null)
                                {
                                    //using (FileStream fileStream = File.Open("TestInsertedContent.docx", FileMode.Open))
                                    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(
                                        AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                                    thisStream.Position = 0;
                                    chunk.FeedData(thisStream);
                                    AltChunk altChunk = new AltChunk();
                                    altChunk.Id = altChunkId;
                                    mainPart.Document
                                        .Body
                                        .InsertAfter(altChunk, mainPart.Document.Body.Elements<Paragraph>().Last());

                                }
                            }
                            idx += 1;
                            altChunkId = "AltChunkId" + idx;
                        }

                    }


                }
                int bLenght = (int)docStreams[0].Length;
                byteArray = new byte[bLenght];
                docStreams[0].Position = 0;
                docStreams[0].Read(byteArray, 0, bLenght);

                logger.WriteGranularTimingMessage("end CreateDocument674");
            }

            return byteArray;
        }

        public static byte[] ByteConcat(byte[] array1, byte[] array2)
        {
            var newSize = array1.Length + array2.Length;
            var ms = new MemoryStream(new byte[newSize], 0, newSize, true, true);
            ms.Write(array1, 0, array1.Length);
            ms.Write(array2, 0, array2.Length);
            return ms.GetBuffer();

        }

        public static byte[] CreateDocument(IOrganizationService service, 
            Entity thisResource, 
            string template, 
            IMCSLogger logger, 
            string altChunkId, 
            int docNumber)
        {
            logger.setMethod = "CreateDocument";
            logger.WriteGranularTimingMessage("start CreateDocument");


            var recordGuid = thisResource.Id;
            byte[] bytes;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                crme_docgendocument d = DocGen.GetDocGenDocument(service, template, logger);

                System.Collections.Generic.List<crme_docgenfieldformat> fieldFormat = DocGen.GetFieldFormat(service, logger);
                System.Collections.Generic.List<crme_docgendocumentfield> documentFields = DocGen.GetDocGenDocumentFields(d.Id, service, logger);
                System.Collections.Generic.List<crme_docgentablemapping> dynamicTableMappings = DocGen.GetDocGenDynamicTableMappings(d.Id, service, logger);
                System.Collections.Generic.List<crme_docgentablemapping> staticTableMappings = DocGen.GetDocGenStaticTableMappings(d.Id, service, logger);

                font = d.crme_Font;
                fontsize = d.crme_FontSize;
                //Get FetchXml statement for this document and get the information from CRM.

                if (d.crme_DynamictablesOnly.Value)
                {
                    if (!DynamicDataExists(dynamicTableMappings, logger, thisResource.Id, service))
                    {
                        return null;
                    }
                }
                System.Collections.Generic.List<completeDocumentFields> completedDocFields = new List<completeDocumentFields>();
                //this will only return data if we are NOT doing just dynamic only tables.
                if (!d.crme_DynamictablesOnly.Value)
                {
                    string fetchXml = String.Format(d.crme_DocGenQuery, thisResource.Id.ToString());
                    Entity e = DocGen.FetchPrimaryXml(service, fetchXml, logger);

                    //merge the formats, the data from CRM, and the fields that this document has together.  
                    //Resulting in a List object that has the bookmark name, the CRM name, which isn't really relevant anymore, the data, and the format

                    completedDocFields = completeDocumentFields.PopulateFields(e, fieldFormat, documentFields);

                    //For static tables, the data can be done 2 ways.  Either as tr_ or tc_ bookmarks, designating a table, or just like normal fields.  Since they are 
                    //static, we know that there will only be so many of them.  The tableMapping entity actually tells us where to start and stop
                    //if we get multiple rows back.
                    //This just merges the data from the tables with the data from the document.
                    //during this process, the index is appended, so crme_ssn, becomes crme_ssn0.
                    completedDocFields = completeDocumentFields.PopulateTableFields(completedDocFields,
                        staticTableMappings, fieldFormat, service, e.Id, logger, documentFields);
                }
                //completedDocFields = completeDocumentFields.PopulateTableFields(completedDocFields, dynamicTableMappings, fieldFormat, service, e.Id, logger, documentFields);
                bytes = DocGen.ReplaceBookMarks(service, documentFields, completedDocFields, dynamicTableMappings, staticTableMappings, template, thisResource.Id, logger, d.Id, altChunkId, docNumber);

            }

            logger.WriteGranularTimingMessage("end CreateDocument");

            return bytes;
        }
        public static MemoryStream RCCreateDocument(IOrganizationService service, Entity thisResource, string template, IMCSLogger logger, string altChunkId, int docNumber)
        {
            logger.setMethod = "CreateDocument";
            logger.WriteGranularTimingMessage("start CreateDocument");
            

            var recordGuid = thisResource.Id;
            MemoryStream thisStream;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                crme_docgendocument d = DocGen.GetDocGenDocument(service, template, logger);

                System.Collections.Generic.List<crme_docgenfieldformat> fieldFormat = DocGen.GetFieldFormat(service, logger);
                System.Collections.Generic.List<crme_docgendocumentfield> documentFields = DocGen.GetDocGenDocumentFields(d.Id, service, logger);
                System.Collections.Generic.List<crme_docgentablemapping> dynamicTableMappings = DocGen.GetDocGenDynamicTableMappings(d.Id, service, logger);
                System.Collections.Generic.List<crme_docgentablemapping> staticTableMappings = DocGen.GetDocGenStaticTableMappings(d.Id, service, logger);

                font = d.crme_Font;
                fontsize = d.crme_FontSize;
                //Get FetchXml statement for this document and get the information from CRM.

                if (d.crme_DynamictablesOnly.Value)
                {
                    if (!DynamicDataExists(dynamicTableMappings, logger, thisResource.Id, service))
                    {
                        return null;
                    }
                }
                System.Collections.Generic.List<completeDocumentFields> completedDocFields = new List<completeDocumentFields>();
                //this will only return data if we are NOT doing just dynamic only tables.
                if (!d.crme_DynamictablesOnly.Value)
                {
                    //this will only return data if we are NOT doing just dynamic only tables.
                    string fetchXml = String.Format(d.crme_DocGenQuery, thisResource.Id.ToString());
                    Entity e = DocGen.FetchPrimaryXml(service, fetchXml, logger);

                    //merge the formats, the data from CRM, and the fields that this document has together.  
                    //Resulting in a List object that has the bookmark name, the CRM name, which isn't really relevant anymore, the data, and the format

                    completedDocFields = completeDocumentFields.PopulateFields(e, fieldFormat, documentFields);

                    //For static tables, the data can be done 2 ways.  Either as tr_ or tc_ bookmarks, designating a table, or just like normal fields.  Since they are 
                    //static, we know that there will only be so many of them.  The tableMapping entity actually tells us where to start and stop
                    //if we get multiple rows back.
                    //This just merges the data from the tables with the data from the document.
                    //during this process, the index is appended, so crme_ssn, becomes crme_ssn0.
                    completedDocFields = completeDocumentFields.PopulateTableFields(completedDocFields,
                        staticTableMappings, fieldFormat, service, e.Id, logger, documentFields);
                }
                //completedDocFields = completeDocumentFields.PopulateTableFields(completedDocFields, dynamicTableMappings, fieldFormat, service, e.Id, logger, documentFields);
                thisStream = DocGen.RCReplaceBookMarks(service, documentFields, completedDocFields, dynamicTableMappings, staticTableMappings, template, thisResource.Id, logger, d.Id, altChunkId, docNumber);

            }

            logger.WriteGranularTimingMessage("end CreateDocument");

            return thisStream;
        }

        public static MemoryStream CreateDocument674(IOrganizationService service, Guid depID, string template, IMCSLogger logger, string altChunkId, int docNumber)
        {
            logger.setMethod = "CreateDocument674";
            logger.WriteGranularTimingMessage("start CreateDocument674");


            //var recordGuid = thisResource.Id;
            var recordGuid = depID;
            MemoryStream thisStream;

            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                crme_docgendocument d = DocGen.GetDocGenDocument(service, template, logger);

                System.Collections.Generic.List<crme_docgenfieldformat> fieldFormat = DocGen.GetFieldFormat(service, logger);
                System.Collections.Generic.List<crme_docgendocumentfield> documentFields = DocGen.GetDocGenDocumentFields(d.Id, service, logger);
                System.Collections.Generic.List<crme_docgentablemapping> dynamicTableMappings = DocGen.GetDocGenDynamicTableMappings(d.Id, service, logger);
                System.Collections.Generic.List<crme_docgentablemapping> staticTableMappings = DocGen.GetDocGenStaticTableMappings(d.Id, service, logger);

                font = d.crme_Font;
                fontsize = d.crme_FontSize;
                //Get FetchXml statement for this document and get the information from CRM.

                if (d.crme_DynamictablesOnly.Value)
                {
                    if (!DynamicDataExists(dynamicTableMappings, logger, recordGuid, service))
                    {
                        return null;
                    }
                }
                System.Collections.Generic.List<completeDocumentFields> completedDocFields = new List<completeDocumentFields>();
                //this will only return data if we are NOT doing just dynamic only tables.
                if (!d.crme_DynamictablesOnly.Value)
                {
                    //this will only return data if we are NOT doing just dynamic only tables.
                    string fetchXml = String.Format(d.crme_DocGenQuery, recordGuid.ToString());
                    Entity e = DocGen.FetchPrimaryXml(service, fetchXml, logger);

                    //merge the formats, the data from CRM, and the fields that this document has together.  
                    //Resulting in a List object that has the bookmark name, the CRM name, which isn't really relevant anymore, the data, and the format

                    completedDocFields = completeDocumentFields.PopulateFields(e, fieldFormat, documentFields);

                    //For static tables, the data can be done 2 ways.  Either as tr_ or tc_ bookmarks, designating a table, or just like normal fields.  Since they are 
                    //static, we know that there will only be so many of them.  The tableMapping entity actually tells us where to start and stop
                    //if we get multiple rows back.
                    //This just merges the data from the tables with the data from the document.
                    //during this process, the index is appended, so crme_ssn, becomes crme_ssn0.
                    completedDocFields = completeDocumentFields.PopulateTableFields(completedDocFields,
                        staticTableMappings, fieldFormat, service, e.Id, logger, documentFields);
                }
                //completedDocFields = completeDocumentFields.PopulateTableFields(completedDocFields, dynamicTableMappings, fieldFormat, service, e.Id, logger, documentFields);
                thisStream = DocGen.RCReplaceBookMarks(service, documentFields, completedDocFields, dynamicTableMappings, staticTableMappings, template, recordGuid, logger, d.Id, altChunkId, docNumber);

            }

            logger.WriteGranularTimingMessage("end CreateDocument674");

            return thisStream;
        }

        public static bool DynamicDataExists(List<crme_docgentablemapping> dynamicTableMappingList, IMCSLogger logger, Guid primaryEntityGuid, IOrganizationService service)
        {
            logger.setMethod = "DynamicDataExists";
            logger.WriteGranularTimingMessage("start DynamicDataExists");

            foreach (var crmeDocgentablemapping in dynamicTableMappingList)
            {
                //Get the fetchxml for this table.
                string fetchXml = String.Format(crmeDocgentablemapping.crme_TableQuery,
                    primaryEntityGuid.ToString());
                EntityCollection ec = FetchTableXml(service, fetchXml, logger);

                //Loop through the entities
                //Need to make sure we only dso this if the rows are right
                var startingRow = (crmeDocgentablemapping.crme_StartingRow != null)
                    ? crmeDocgentablemapping.crme_StartingRow.Value
                    : 0;
                var endingRow = (crmeDocgentablemapping.crme_MaxNumberofRows != null)
                    ? crmeDocgentablemapping.crme_MaxNumberofRows.Value + startingRow - 1
                    : 99;
                var currentRow = 0;
                startingRow = startingRow - 1;
                #region - loop through records returned with fetch

                foreach (var entity in ec.Entities)
                {
                    if (currentRow >= startingRow)
                    {
                        if (currentRow <= endingRow)
                        {
                            //all we need is 1 row that fits in order to proceed with the code
                            return true;
                        }
                    }
                    currentRow += 1;
                }

                #endregion

            }
            return false;
        }

        public static byte[] DOnReplaceBookMarks(IOrganizationService service, List<crme_docgendocumentfield> docFieldsList, List<completeDocumentFields> completedDocFieldsList, List<crme_docgentablemapping> dynamicTableMappingList, List<crme_docgentablemapping> statictableMappingList, string template, Guid primaryEntityGuid, IMCSLogger logger, Guid DocID)
        {

            logger.setMethod = "ReplaceBookMarks";
            logger.WriteGranularTimingMessage("start ReplaceBookMarks");

            byte[] byteArray = null;


            MemoryStream docStream = GetWordProcessingDocument(service, template, logger);
            using (WordprocessingDocument wordPackage = WordprocessingDocument.Open(docStream, true))
            {

                MainDocumentPart part = wordPackage.MainDocumentPart;



                #region dynamics table mapping
                foreach (var crmeDocgentablemapping in dynamicTableMappingList)
                {
                    //Since there could be continuation tables, get all tables that start with the name.  The first one should be fixed length and the next will be self expanding.
                    var tableBookMark = part.Document.Body.Descendants<BookmarkStart>().Where(m => m.Name.Value.StartsWith(crmeDocgentablemapping.crme_name)).ToList();

                    if (tableBookMark.Count > 0)
                    {
                        //The bookmark is embeded in the header of the table.  Look back to get the entire table.
                        Table table = tableBookMark[0].Ancestors<Table>().FirstOrDefault();

                        TableRow contRow = null;

                        //Get the fetchxml for this table.
                        string fetchXml = String.Format(crmeDocgentablemapping.crme_TableQuery,
                            primaryEntityGuid.ToString());
                        EntityCollection ec = FetchTableXml(service, fetchXml, logger);

                        //Keep an index so we can look the fixed tables up by a specific row index (ex.  tr_row0, tr_row1, etc...)
                        int rowIdx = 0;


                        //Get the fields that are defined for this table.
                        var tableFields = DocGen.GetDocGenTableFields(crmeDocgentablemapping.Id, service, logger);

                        TableRow tableRow = null;

                        //Loop through the entities
                        foreach (var entity in ec.Entities)
                        {
                            //Find ROW Bookmark, this appends the idx so that we can locate a specific row.
                            //string rowId = crmeDocgentablemapping.new_TableRowName + rowIdx.ToString(); - old
                            string rowId = crmeDocgentablemapping.new_TableRowName.ToString();
                            var tableRowBookMark =
                                table.Descendants<BookmarkStart>().Where(b => b.Name.Value.Equals(rowId)).FirstOrDefault();


                            //If this is true hopefully we are the end of the first table.
                            if (tableRowBookMark == null)
                            {
                                if (contRow == null)
                                {
                                    //Check to see if we have another table to continue on
                                    if (tableBookMark.Count() == 2)
                                    {
                                        //Get the next table
                                        table = tableBookMark[0].Ancestors<Table>().FirstOrDefault();
                                        tableRowBookMark =
                                            table.Descendants<BookmarkStart>().Where(b => b.Name.Value.Equals(rowId)).FirstOrDefault();
                                        //rc NOT SURE ABOUT THIS
                                        if (tableRowBookMark != null)
                                        {
                                            tableRow = tableRowBookMark.Ancestors<TableRow>().FirstOrDefault();
                                            tableRowBookMark.Remove();

                                            //Clone the current row, so that we can insert additonal rows, since the 2nd table will be dynamic.
                                            contRow = (TableRow)tableRow.Clone();
                                        }
                                    }
                                    else
                                    {
                                        //we HAVE A PROBLEM
                                        rowId = crmeDocgentablemapping.new_TableRowName + (rowIdx - 1).ToString();
                                        tableRowBookMark =
                                           table.Descendants<BookmarkStart>().Where(b => b.Name.Value.Equals(rowId)).FirstOrDefault();
                                        //rc NOT SURE ABOUT THIS
                                        if (tableRowBookMark != null)
                                        {
                                            tableRow = tableRowBookMark.Ancestors<TableRow>().FirstOrDefault();
                                            tableRowBookMark.Remove();

                                            //Clone the current row, so that we can insert additonal rows, since the 2nd table will be dynamic.
                                            contRow = (TableRow)tableRow.Clone();
                                        }

                                    }
                                }
                                else
                                {
                                    //Building a new dynamic row.  Remove existing bookmarks from the previous row, as we can't have bookmarks with the same name in the document.
                                    if (tableRow != null)
                                    {
                                        var tcBookmarksStartToRemove = tableRow.Descendants<BookmarkStart>().ToList();
                                        if (tcBookmarksStartToRemove != null)
                                        {
                                            foreach (var b in tcBookmarksStartToRemove)
                                            {
                                                b.Remove();
                                            }

                                            if (tableRow != null)
                                            {
                                                var tcBookmarksEndToRemove = tableRow.Descendants<BookmarkEnd>().ToList();
                                                foreach (var b in tcBookmarksEndToRemove)
                                                {
                                                    b.Remove();
                                                }
                                            }
                                        }


                                        //Insert a new row.
                                        tableRow.InsertAfterSelf(contRow);
                                        tableRow = contRow;

                                        //Clone the current row, in case we need to insert more.
                                        contRow = (TableRow)tableRow.Clone();
                                    }
                                }
                            }
                            else
                            {
                                tableRow = tableRowBookMark.Ancestors<TableRow>().FirstOrDefault();
                            }

                            if (tableRow != null)
                            {
                                var tableCellBookMark =
                                    tableRow.Descendants<BookmarkStart>().Where(c => c.Name.Value.StartsWith("tc_"));

                                System.Collections.Generic.List<completeDocumentFields> cellFields = completeDocumentFields.PopulateFields(entity, DocGen.GetFieldFormat(service, logger), DocGen.GetDocGenDocumentFields(DocID, service, logger));


                                foreach (var cellBookMark in tableCellBookMark)
                                {
                                    ReplaceCellBookMark(service, tableFields, cellFields, cellBookMark, logger, docFieldsList);
                                    //ReplaceBookMark(service, tableFields, cellFields, cellBookMark, logger);

                                }
                            }

                            rowIdx++;
                        }

                    }


                }
                #endregion


                #region Main Replacement
                var res = from bm in part.Document.Body.Descendants<BookmarkStart>()
                          where !(bm.Name.Value.StartsWith("tr_") || bm.Name.Value.StartsWith("tc_"))
                          select bm;

                foreach (var bookMark in res)
                {
                    if (bookMark.Name != "_GoBack")
                    {
                        ReplaceBookMark(service, docFieldsList, completedDocFieldsList, bookMark, logger);
                    }
                }
                #endregion

            }

            int bLenght = (int)docStream.Length;
            byteArray = new byte[bLenght];
            docStream.Position = 0;
            docStream.Read(byteArray, 0, bLenght);

            logger.WriteGranularTimingMessage("end ReplaceBookMarks");

            return byteArray;
        }
        public static byte[] ReplaceBookMarks(IOrganizationService service, 
            List<crme_docgendocumentfield> docFieldsList, 
            List<completeDocumentFields> completedDocFieldsList, 
            List<crme_docgentablemapping> dynamicTableMappingList, 
            List<crme_docgentablemapping> statictableMappingList, 
            string template, 
            Guid primaryEntityGuid, 
            IMCSLogger logger, 
            Guid DocID, 
            string altChunkId, 
            int docNumber)
        {

            logger.setMethod = "ReplaceBookMarks";
            logger.WriteGranularTimingMessage("start ReplaceBookMarks");

            byte[] byteArray = null;


            MemoryStream docStream = GetWordProcessingDocument(service, template, logger);
            using (WordprocessingDocument wordPackage = WordprocessingDocument.Open(docStream, true))
            {

                MainDocumentPart part = wordPackage.MainDocumentPart;

                #region dynamics table mapping

                foreach (var crmeDocgentablemapping in dynamicTableMappingList)
                {

                    //Since there could be continuation tables, get all tables that start with the name.  The first one should be fixed length and the next will be self expanding.
                    var tableBookMark =
                        part.Document.Body.Descendants<BookmarkStart>()
                            .Where(m => m.Name.Value.StartsWith(crmeDocgentablemapping.crme_name))
                            .ToList();

                    if (tableBookMark.Count > 0)
                    {
                        //The bookmark is embeded in the header of the table.  Look back to get the entire table.
                        Table table = tableBookMark[0].Ancestors<Table>().FirstOrDefault();

                        TableRow contRow = null;

                        //Get the fetchxml for this table.
                        string fetchXml = String.Format(crmeDocgentablemapping.crme_TableQuery,
                            primaryEntityGuid.ToString());
                        EntityCollection ec = FetchTableXml(service, fetchXml, logger);

                        //Keep an index so we can look the fixed tables up by a specific row index (ex.  tr_row0, tr_row1, etc...)


                        //Get the fields that are defined for this table.
                        var tableFields = DocGen.GetDocGenTableFields(crmeDocgentablemapping.Id, service, logger);

                        TableRow tableRow = null;

                        var totalRows = ec.Entities.Count;
                        //Loop through the entities
                        //Need to make sure we only dso this if the rows are right
                        var startingRow = (crmeDocgentablemapping.crme_StartingRow != null)
                            ? crmeDocgentablemapping.crme_StartingRow.Value
                            : 0;
                        var endingRow = (crmeDocgentablemapping.crme_MaxNumberofRows != null)
                            ? crmeDocgentablemapping.crme_MaxNumberofRows.Value + startingRow - 1
                            : 99;
                        startingRow = startingRow - 1;
                        var currentRow = 0;
                        var idx = 1;
                        #region - loop through records returned with fetch

                        foreach (var entity in ec.Entities)
                        {
                            if (currentRow >= startingRow)
                            {
                                if (currentRow <= endingRow)
                                {
                                    //Find ROW Bookmark, this appends the idx so that we can locate a specific row.
                                    //string rowId = crmeDocgentablemapping.new_TableRowName + rowIdx.ToString(); - old
                                    string rowId = crmeDocgentablemapping.new_TableRowName.ToString();
                                    var tableRowBookMark =
                                        table.Descendants<BookmarkStart>()
                                            .Where(b => b.Name.Value.Equals(rowId))
                                            .FirstOrDefault();

                                    #region got tablebookmark

                                    if (tableRowBookMark != null)
                                    {
                                        tableRow = tableRowBookMark.Ancestors<TableRow>().FirstOrDefault();
                                        if (tableRow != null)
                                        {
                                            if (totalRows > idx)
                                            {
                                                contRow = (TableRow)tableRow.Clone();
                                                tableRow.InsertAfterSelf(contRow);
                                            }
                                            var tableCellBookMark =
                                                tableRow.Descendants<BookmarkStart>()
                                                    .Where(c => c.Name.Value.StartsWith("tc_"));

                                            System.Collections.Generic.List<completeDocumentFields> cellFields =
                                                completeDocumentFields.PopulateDynamicCellFields(entity.Attributes,
                                                    entity.LogicalName,
                                                    DocGen.GetFieldFormat(service, logger),
                                                    DocGen.GetDocGenTableFields(crmeDocgentablemapping.Id, service,
                                                        logger));


                                            foreach (var cellBookMark in tableCellBookMark)
                                            {
                                                ReplaceCellBookMark(service, tableFields, cellFields, cellBookMark,
                                                    logger,
                                                    docFieldsList);

                                            }

                                            tableRowBookMark.Remove();

                                            idx += 1;
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                }


                #endregion


                #region Main Replacement
                var res = from bm in part.Document.Body.Descendants<BookmarkStart>()
                          where !(bm.Name.Value.StartsWith("tr_") || bm.Name.Value.StartsWith("tc_"))
                          select bm;

                foreach (var bookMark in res)
                {
                    if (bookMark.Name != "_GoBack")
                    {
                        ReplaceBookMark(service, docFieldsList, completedDocFieldsList, bookMark, logger);
                    }
                }
                #endregion

            }

            int bLenght = (int)docStream.Length;
            byteArray = new byte[bLenght];
            docStream.Position = 0;
            docStream.Read(byteArray, 0, bLenght);

            logger.WriteGranularTimingMessage("end ReplaceBookMarks");

            return byteArray;
        }
        public static MemoryStream RCReplaceBookMarks(IOrganizationService service, List<crme_docgendocumentfield> docFieldsList, List<completeDocumentFields> completedDocFieldsList, List<crme_docgentablemapping> dynamicTableMappingList, List<crme_docgentablemapping> statictableMappingList, string template, Guid primaryEntityGuid, IMCSLogger logger, Guid DocID, string altChunkId, int docNumber)
        {

            logger.setMethod = "ReplaceBookMarks";
            logger.WriteGranularTimingMessage("start ReplaceBookMarks");


            MemoryStream docStream = GetWordProcessingDocument(service, template, logger);
            using (WordprocessingDocument wordPackage = WordprocessingDocument.Open(docStream, true))
            {

                MainDocumentPart part = wordPackage.MainDocumentPart;

                #region dynamics table mapping

                foreach (var crmeDocgentablemapping in dynamicTableMappingList)
                {

                    //Since there could be continuation tables, get all tables that start with the name.  The first one should be fixed length and the next will be self expanding.
                    var tableBookMark =
                        part.Document.Body.Descendants<BookmarkStart>()
                            .Where(m => m.Name.Value.StartsWith(crmeDocgentablemapping.crme_name))
                            .ToList();

                    if (tableBookMark.Count > 0)
                    {
                        //The bookmark is embeded in the header of the table.  Look back to get the entire table.
                        Table table = tableBookMark[0].Ancestors<Table>().FirstOrDefault();

                        TableRow contRow = null;

                        //Get the fetchxml for this table.
                        string fetchXml = String.Format(crmeDocgentablemapping.crme_TableQuery,
                            primaryEntityGuid.ToString());
                        EntityCollection ec = FetchTableXml(service, fetchXml, logger);

                        //Keep an index so we can look the fixed tables up by a specific row index (ex.  tr_row0, tr_row1, etc...)


                        //Get the fields that are defined for this table.
                        var tableFields = DocGen.GetDocGenTableFields(crmeDocgentablemapping.Id, service, logger);

                        TableRow tableRow = null;

                        var totalRows = ec.Entities.Count;
                        //Loop through the entities
                        //Need to make sure we only dso this if the rows are right
                        var startingRow = (crmeDocgentablemapping.crme_StartingRow != null)
                            ? crmeDocgentablemapping.crme_StartingRow.Value
                            : 0;
                        var endingRow = (crmeDocgentablemapping.crme_MaxNumberofRows != null)
                            ? crmeDocgentablemapping.crme_MaxNumberofRows.Value + startingRow - 1
                            : 99;
                        startingRow = startingRow - 1;
                        var currentRow = 0;
                        var idx = 1;
                        #region - loop through records returned with fetch

                        foreach (var entity in ec.Entities)
                        {
                            if (currentRow >= startingRow)
                            {
                                if (currentRow <= endingRow)
                                {
                                    //Find ROW Bookmark, this appends the idx so that we can locate a specific row.
                                    //string rowId = crmeDocgentablemapping.new_TableRowName + rowIdx.ToString(); - old
                                    string rowId = crmeDocgentablemapping.new_TableRowName.ToString();
                                    var tableRowBookMark =
                                        table.Descendants<BookmarkStart>()
                                            .Where(b => b.Name.Value.Equals(rowId))
                                            .FirstOrDefault();

                                    #region got tablebookmark

                                    if (tableRowBookMark != null)
                                    {
                                        tableRow = tableRowBookMark.Ancestors<TableRow>().FirstOrDefault();
                                        if (tableRow != null)
                                        {
                                            if (totalRows > idx)
                                            {
                                                contRow = (TableRow)tableRow.Clone();
                                                tableRow.InsertAfterSelf(contRow);
                                            }
                                            var tableCellBookMark =
                                                tableRow.Descendants<BookmarkStart>()
                                                    .Where(c => c.Name.Value.StartsWith("tc_"));

                                            System.Collections.Generic.List<completeDocumentFields> cellFields =
                                                completeDocumentFields.PopulateDynamicCellFields(entity.Attributes,
                                                    entity.LogicalName,
                                                    DocGen.GetFieldFormat(service, logger),
                                                    DocGen.GetDocGenTableFields(crmeDocgentablemapping.Id, service,
                                                        logger));


                                            foreach (var cellBookMark in tableCellBookMark)
                                            {
                                                ReplaceCellBookMark(service, tableFields, cellFields, cellBookMark,
                                                    logger,
                                                    docFieldsList);

                                            }

                                            tableRowBookMark.Remove();
                                        }

 
                                    }

                                    #endregion
                                }
                            }

                            currentRow += 1;

                            idx += 1;
                        }

                        #endregion
                    }
                }
                
                #endregion


                #region Main Replacement
                var res = from bm in part.Document.Body.Descendants<BookmarkStart>()
                          where !(bm.Name.Value.StartsWith("tr_") || bm.Name.Value.StartsWith("tc_"))
                          select bm;

                foreach (var bookMark in res)
                {
                    if (bookMark.Name != "_GoBack")
                    {
                        ReplaceBookMark(service, docFieldsList, completedDocFieldsList, bookMark, logger);
                    }
                }
                #endregion

            }


            logger.WriteGranularTimingMessage("end ReplaceBookMarks");

            return docStream;
        }

        private enum FIELD_OUTPUT_TYPE { Text = 935950000, CheckBox = 935950001 }


        private static void ReplaceBookMark(IOrganizationService service, List<crme_docgendocumentfield> docFieldsList, List<completeDocumentFields> completedDocFieldsList, BookmarkStart bookMark, IMCSLogger logger)
        {
            logger.setMethod = "ReplaceBookMark";
            logger.WriteGranularTimingMessage("start ReplaceBookMark");

            string fieldValue = bookMark.Name;


            completeDocumentFields fieldtoGet = completeDocumentFields.GetField(fieldValue, docFieldsList, completedDocFieldsList);
            if (fieldtoGet != null)
            {

                var conversion = docFieldsList.Where(c => c.crme_name.Equals(bookMark.Name)).FirstOrDefault();


                if (conversion != null && conversion.crme_FieldOutputType != null &&
                    conversion.crme_FieldOutputType.Value != (int)FIELD_OUTPUT_TYPE.Text)
                {

                    //CheckBox
                    if (conversion.crme_FieldOutputType.Value == (int)FIELD_OUTPUT_TYPE.CheckBox)
                    {
                        CheckBox cb = bookMark.PreviousSibling().Descendants<CheckBox>().FirstOrDefault();
                        if (cb != null)
                        {
                            Checked c = cb.Descendants<Checked>().FirstOrDefault();
                            if (c == null)
                            {
                                c = cb.AppendChild<Checked>(new Checked());
                            }

                            c.Val = OnOffValue.FromBoolean(false);
                            if (FormatField(service, fieldtoGet, logger, docFieldsList, completedDocFieldsList).Equals(conversion.crme_Value))
                            {
                                c.Val = OnOffValue.FromBoolean(true);
                            }

                        }
                        else
                        {
                            //TODO: THROW ERROR
                        }

                    }
                }
                else //Default to Text
                {


                    OpenXmlElement nextSibling = bookMark.NextSibling();
                    Debug.WriteLine(string.Format("Type:  {0}", nextSibling.GetType().Name));

                    TextInput textInput = bookMark.PreviousSibling().Descendants<TextInput>().FirstOrDefault();
                    if (textInput != null)
                    {
                        DefaultTextBoxFormFieldString defaultTextBox =
                            textInput.Descendants<DefaultTextBoxFormFieldString>().FirstOrDefault();
                        if (defaultTextBox == null)
                        {
                            defaultTextBox = new DefaultTextBoxFormFieldString();
                            textInput.Append(defaultTextBox);
                        }
                        defaultTextBox.Val = FormatField(service, fieldtoGet, logger, docFieldsList, completedDocFieldsList);
                        //textInput.Append(new Text(FormatField(service, fieldtoGet, logger)));
                        var foundField = false;
                        while (nextSibling.GetType().Name != "BookmarkEnd")
                        {
                            Text text = nextSibling.Descendants<Text>().FirstOrDefault();

                            if (text != null)
                            {
                                if (!foundField)
                                {
                                    text.Text = FormatField(service, fieldtoGet, logger, docFieldsList,
                                        completedDocFieldsList);
                                    foundField = true;

                                }
                                else
                                {
                                    text.Text = "";
                                }
                                // break;
                            }
                            nextSibling = nextSibling.NextSibling();
                        }

                    }
                    else
                    {
                        //not a text box
                        Text texttoPop = bookMark.PreviousSibling().Descendants<Text>().FirstOrDefault();
                        if (texttoPop != null)
                        {
                            texttoPop.Text = FormatField(service, fieldtoGet, logger, docFieldsList,
                                completedDocFieldsList);
                        }
                        else
                        {

                            Run run1 = new Run();

                            RunProperties runProperties1 = new RunProperties();
                            RunFonts runFonts1 = new RunFonts() { ComplexScript = font };
                            FontSize fontSize1 = new FontSize() { Val = fontsize };
                            FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = fontsize };

                            runProperties1.Append(runFonts1);
                            runProperties1.Append(fontSize1);
                            runProperties1.Append(fontSizeComplexScript1);
                            Text text1 = new Text();
                            text1.Text = FormatField(service, fieldtoGet, logger, docFieldsList, completedDocFieldsList);

                            run1.Append(runProperties1);
                            run1.Append(text1);

                            bookMark.Parent.InsertAfter(run1, bookMark);

                        }
                    }



                }
            }

            logger.WriteGranularTimingMessage("end ReplaceBookMark");
        }
        private static void ReplaceCellBookMark(IOrganizationService service, List<crme_docgentablefields> tableFieldsList, List<completeDocumentFields> completedDocFieldsList, BookmarkStart bookMark, IMCSLogger logger, List<crme_docgendocumentfield> docFieldsList)
        {
            logger.setMethod = "ReplaceCellBookMark";
            logger.WriteGranularTimingMessage("start ReplaceCellBookMark");

            string fieldValue = bookMark.Name;
            completeDocumentFields fieldtoGet = completeDocumentFields.GetCellField(fieldValue, tableFieldsList, completedDocFieldsList);

            var conversion = tableFieldsList.Where(c => bookMark.Name.ToString().StartsWith(c.crme_name)).FirstOrDefault();


            if (conversion != null && conversion.crme_FieldOutputType != null && conversion.crme_FieldOutputType.Value != (int)FIELD_OUTPUT_TYPE.Text)
            {

                //CheckBox
                if (conversion.crme_FieldOutputType.Value == (int)FIELD_OUTPUT_TYPE.CheckBox)
                {
                    CheckBox cb = bookMark.PreviousSibling().Descendants<CheckBox>().FirstOrDefault();
                    if (cb != null)
                    {
                        Checked c = cb.Descendants<Checked>().FirstOrDefault();
                        if (c == null)
                        {
                            c = cb.AppendChild<Checked>(new Checked());
                        }

                        c.Val = OnOffValue.FromBoolean(false);
                        if (FormatField(service, fieldtoGet, logger, docFieldsList, completedDocFieldsList).Equals(conversion.crme_Value))
                        {
                            c.Val = OnOffValue.FromBoolean(true);
                        }

                    }
                    else
                    {
                        //TODO: THROW ERROR
                    }

                }
            }
            else  //Default to Text
            {
                OpenXmlElement nextSibling = bookMark.NextSibling();
                Debug.WriteLine(string.Format("Type:  {0}", nextSibling.GetType().Name));

                TextInput textInput = bookMark.PreviousSibling().Descendants<TextInput>().FirstOrDefault();
                if (textInput != null)
                {
                    DefaultTextBoxFormFieldString defaultTextBox =
                        textInput.Descendants<DefaultTextBoxFormFieldString>().FirstOrDefault();
                    if (defaultTextBox == null)
                    {
                        defaultTextBox = new DefaultTextBoxFormFieldString();
                        textInput.Append(defaultTextBox);
                    }
                    defaultTextBox.Val = FormatField(service, fieldtoGet, logger, docFieldsList, completedDocFieldsList);

                    while (nextSibling.GetType().Name != "BookmarkEnd")
                    {
                        Text text = nextSibling.Descendants<Text>().FirstOrDefault();

                        if (text != null)
                        {
                            text.Text = FormatField(service, fieldtoGet, logger, docFieldsList, completedDocFieldsList);
                            break;
                        }

                        nextSibling = nextSibling.NextSibling();
                    }
                }

                else
                {
                    //not a text box
                    Text texttoPop = bookMark.PreviousSibling().Descendants<Text>().FirstOrDefault();
                    if (texttoPop != null)
                    {
                        texttoPop.Text = FormatField(service, fieldtoGet, logger, docFieldsList,
                            completedDocFieldsList);
                    }
                    else
                    {
                        // bookMark.InnerText = FormatField(service, fieldtoGet, logger, docFieldsList,completedDocFieldsList);
                    }

                }

            }
            //while (!(nextSibling == null || nextSibling.GetType().Name == "BookmarkEnd"))
            //{
            //    foreach (var c in nextSibling.Descendants<Text>())
            //    {
            //        Debug.WriteLine(string.Format("Child: {0}", c.GetType().Name));
            //        if (c.GetType().Name == "Text")
            //        {
            //            if (!firstTextFound)
            //            {
            //                firstTextFound = true;
            //                ((Text)c).Text = FormatField(service, fieldtoGet, logger);
            //                //((Text)c).Text = fieldValue;   //Test to replace all bookmarks with there bookmark name.
            //            }
            //            else
            //            {
            //                //removeParent = true;
            //                break;
            //            }
            //        }
            //    }
            //    nextSibling = nextSibling.NextSibling();

            //    if (nextSibling != null) Debug.WriteLine(string.Format("Type:  {0}", nextSibling.GetType().Name));
            //}

            logger.WriteGranularTimingMessage("end ReplaceCellBookMark");
        }

        public static string FormatField(IOrganizationService service, completeDocumentFields fieldtoGet, IMCSLogger logger, List<crme_docgendocumentfield> docFieldsList, List<completeDocumentFields> completedDocFieldsList)
        {
            logger.setMethod = "FormatField";
            logger.WriteGranularTimingMessage("start FormatField");
            //need to figure out what the techinical field name is for field 42 for entity with an OTC of 1.


            string retString = string.Empty;

            if (fieldtoGet != null)
            {
                switch (fieldtoGet.dataType)
                {
                    case "string":
                        switch (fieldtoGet.format)
                        {
                            case "as is":
                                var type1 = fieldtoGet.actualValue.GetType();
                                if (type1 == typeof(Microsoft.Xrm.Sdk.AliasedValue))
                                {
                                    AliasedValue retString1 = (AliasedValue)fieldtoGet.actualValue;
                                    retString = retString1.Value.ToString();

                                }
                                else
                                {
                                    retString = fieldtoGet.actualValue.ToString();
                                }
                                //retString = fieldtoGet.actualValue.ToString();
                                break;
                            case "stringssn":
                                retString = String.Format("{0:000-00-0000}", Convert.ToInt32(fieldtoGet.actualValue));
                                break;
                            case "intssn":
                                retString = String.Format("{0:000-00-0000}", fieldtoGet.actualValue);
                                break;
                            case "ucase":
                                var type = fieldtoGet.actualValue.GetType();
                                if(type == typeof(Microsoft.Xrm.Sdk.AliasedValue))
                                {
                                    AliasedValue retString1 = (AliasedValue)fieldtoGet.actualValue;
                                    retString = retString1.Value.ToString();

                                }
                                else
                                {
                                    retString = fieldtoGet.actualValue.ToString().ToUpper();
                                }

                                break;
                            case "stringconcat":
                                retString = GetStringConcatData(service, fieldtoGet, logger, docFieldsList, completedDocFieldsList);
                                break;
                        }
                        break;
                    case "OptionSetValue":
                        var thisOptVal = (OptionSetValue)fieldtoGet.actualValue;
                        retString = GetOptionSetString(service, thisOptVal.Value,
                                                          fieldtoGet.LogicalName,
                                                          fieldtoGet.crmTechnicalName, logger);
                        break;
                    case "Lookup":
                        var thisLookup = (EntityReference)fieldtoGet.actualValue;
                        retString = thisLookup.Name;
                        break;
                    case "bool":
                        var thisBoolVal = (bool)fieldtoGet.actualValue;

                        switch (fieldtoGet.format)
                        {
                            case "True/False":
                                retString = thisBoolVal ? "True" : "False";
                                break;
                            case "true/false":
                                retString = thisBoolVal ? "true" : "false";
                                break;
                            case "Yes/No":
                                retString = thisBoolVal ? "Yes" : "No";
                                break;
                            case "yes/no":
                                retString = thisBoolVal ? "yes" : "no";
                                break;
                            default:
                                break;
                        }
                        break;
                    case "DateTime":
                        DateTime thisDateVal = (DateTime)fieldtoGet.actualValue;
                        if (fieldtoGet.format == null)
                        {
                            retString = thisDateVal.ToLongDateString();
                        }
                        else
                        {
                            retString = String.Format("{0:" + fieldtoGet.format + "}",
                                                         thisDateVal);
                        }
                        break;
                    case "Decimal":
                        // System.Int32 thisIntVal = (System.Int32)fieldtoGet.actualValue;
                        System.Decimal thisIntVal = Convert.ToDecimal(fieldtoGet.actualValue);
                        if (fieldtoGet.format == null)
                        {
                            retString = thisIntVal.ToString();
                        }
                        else
                        {
                            retString = String.Format("{0" + fieldtoGet.format + "}",
                                                         thisIntVal);
                        }
                        break;
                    case "Money":
                        Money thisMoneyVal = (Money)fieldtoGet.actualValue;
                        if (fieldtoGet.format == null)
                        {
                            retString = thisMoneyVal.Value.ToString();
                        }
                        else
                        {
                            retString = String.Format("{0:" + fieldtoGet.format + "}",
                                                         thisMoneyVal.Value);
                        }
                        break;
                    default:
                        retString = fieldtoGet.dataType;
                        break;

                }

            }

            logger.WriteGranularTimingMessage("end FormatField");

            return retString;
        }

        private static string GetStringConcatData(IOrganizationService service, completeDocumentFields fieldtoGet, IMCSLogger logger, List<crme_docgendocumentfield> docFieldsList, List<completeDocumentFields> completedDocFieldsList)
        {
            logger.setMethod = "GetStringConcatData";
            logger.WriteGranularTimingMessage("start GetStringConcatData");

            try
            {


                VRMXRM xrm = new VRMXRM(service);
                var stringConCats = (from docstring in xrm.crme_docgenstringconcentationSet
                                     where docstring.crme_ParentFieldFormat.Id.Equals(fieldtoGet.formatId)
                                     orderby docstring.crme_Sequence
                                     select new
                                     {
                                         docstring.crme_Length,
                                         docstring.crme_ParentFieldFormat,
                                         docstring.crme_StartingPosition,
                                         docstring.crme_crmfield,
                                         docstring.crme_Sequence,
                                         docstring.crme_fieldname,
                                         docstring.crme_separationcharacter

                                     }).ToList();
                var finalField = "";
                foreach (var stringCC in stringConCats)
                {
                    //get the field that we need..
                    completeDocumentFields thisfieldtoGet = completeDocumentFields.GetTemplateField(stringCC.crme_crmfield, docFieldsList, completedDocFieldsList);

                    if (stringCC.crme_Length != null)
                    {
                        if (stringCC.crme_StartingPosition != null)
                        {
                            Microsoft.Xrm.Sdk.AliasedValue thisValue = (Microsoft.Xrm.Sdk.AliasedValue)thisfieldtoGet.actualValue;
                            finalField += thisValue.Value.ToString()
                                .Substring(stringCC.crme_StartingPosition.Value, stringCC.crme_Length.Value);
                        }

                    }
                    else
                    {
                        if (stringCC.crme_separationcharacter != null)
                        {
                            finalField += stringCC.crme_separationcharacter;

                        }
                        else
                        {
                            Microsoft.Xrm.Sdk.AliasedValue thisValue = (Microsoft.Xrm.Sdk.AliasedValue)thisfieldtoGet.actualValue;

                            finalField += thisValue.Value.ToString();
                        }
                    }
                }


                logger.WriteGranularTimingMessage("end GetOptionSetString");

                return finalField;
            }
            catch (Exception ex)
            {
                logger.WriteGranularTimingMessage("Message Text - " + ex.Message.ToString());
                logger.WriteGranularTimingMessage("Inner Exception Text - " + ex.InnerException.ToString());
                return null;
            }
        }


        public static string GetOptionSetString(IOrganizationService service, int optionSetValue, string entityName, string attributeName, IMCSLogger logger)
        {
            logger.setMethod = "GetOptionSetString";
            logger.WriteGranularTimingMessage("start GetOptionSetString");

            try
            {

                string optionSetString = string.Empty;

                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest();
                attributeRequest.EntityLogicalName = entityName;
                attributeRequest.LogicalName = attributeName;
                // Retrieve only the currently published changes, ignoring the changes that have
                // not been published.
                attributeRequest.RetrieveAsIfPublished = true;

                RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);

                // Access the retrieved attribute.
                PicklistAttributeMetadata retrievedAttributeMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
                for (int i = 0; i < retrievedAttributeMetadata.OptionSet.Options.Count; i++)
                {
                    if (retrievedAttributeMetadata.OptionSet.Options[i].Value == optionSetValue)
                    {
                        optionSetString = retrievedAttributeMetadata.OptionSet.Options[i].Label.LocalizedLabels[0].Label;
                        break;
                    }

                }
                logger.WriteGranularTimingMessage("end GetOptionSetString");

                return optionSetString;
            }
            catch (Exception ex)
            {
                logger.WriteGranularTimingMessage("Message Text - " + ex.Message.ToString());
                logger.WriteGranularTimingMessage("Inner Exception Text - " + ex.InnerException.ToString());
                return null;
            }
        }
        public static void SaveDocument(IOrganizationService service, Entity entity, string template, byte[] bytes, bool? convertToPdf, IMCSLogger logger)
        {
            using (VRMXRM serviceContext = new VRMXRM(service))
            {
                Annotation annotation;

                string fileName = string.Format("{0}.docx", template);

                annotation = serviceContext.AnnotationSet.Where(a => a.ObjectId.Id == entity.Id && a.FileName == fileName).FirstOrDefault();

                try
                {
                    if (annotation == null)
                    {
                        Guid newid = Guid.NewGuid();
                        annotation = new Annotation
                        {
                            Id = newid,
                            AnnotationId = newid,
                            Subject = "File Attachment",
                            ObjectTypeCode = entity.LogicalName,
                            DocumentBody = Convert.ToBase64String(bytes),
                            FileName = fileName,
                            MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                        };
                        annotation.ObjectId = new EntityReference
                        {
                            Id = entity.Id,
                            LogicalName = entity.LogicalName
                        };
                        serviceContext.AddObject(annotation);
                    }
                    else
                    {
                        annotation.DocumentBody = Convert.ToBase64String(bytes);
                        serviceContext.UpdateObject(annotation);
                    }
                    //serviceContext.SaveChanges(SaveChangesOptions.ContinueOnError);
                    serviceContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException("Plugin Failed creating document attachment", ex);
                }

            }
        }


    }

    public class completeDocumentFields
    {

        private Guid _formatId;

        public Guid formatId
        {
            get { return _formatId; }
            set { _formatId = value; }
        }
        private string _format;
        public string format
        {
            get { return _format; }
            set { _format = value; }
        }
        private string _logicalName;
        public string LogicalName
        {
            get { return _logicalName; }
            set { _logicalName = value; }
        }
        private string _templateName;

        public string templateName
        {
            get { return _templateName; }
            set { _templateName = value; }
        }
        private string _crmTechnicalName;

        public string crmTechnicalName
        {
            get { return _crmTechnicalName; }
            set { _crmTechnicalName = value; }
        }
        private object _actualValue;

        public object actualValue
        {
            get { return _actualValue; }
            set { _actualValue = value; }
        }
        private string _displayName;

        public string displayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }
        private string _dataType;

        public string dataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        private Guid _relatedEntityId;
        public Guid relatedEntityId
        {
            get { return _relatedEntityId; }
            set { _relatedEntityId = value; }
        }

        public static completeDocumentFields GetField(string bookmarkValue, List<crme_docgendocumentfield> docFields, List<completeDocumentFields> completedDocumentFields)
        {
            foreach (completeDocumentFields fields in completedDocumentFields)
            {
                if (fields.templateName.Equals(bookmarkValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return fields;
                }
            }
            foreach (crme_docgendocumentfield docField in docFields)
            {
                if (docField.crme_name.Equals(bookmarkValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (completeDocumentFields fields in completedDocumentFields)
                    {
                        if (fields.templateName.Equals(docField.crme_CRMField, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return fields;
                        }
                    }
                }
            }
            return null;

        }
        public static completeDocumentFields GetTemplateField(string bookmarkValue, List<crme_docgendocumentfield> docFields, List<completeDocumentFields> completedDocumentFields)
        {
            foreach (completeDocumentFields fields in completedDocumentFields)
            {
                if (fields.crmTechnicalName.Equals(bookmarkValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return fields;
                }
            }
            foreach (crme_docgendocumentfield docField in docFields)
            {
                if (docField.crme_name.Equals(bookmarkValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (completeDocumentFields fields in completedDocumentFields)
                    {
                        if (fields.crmTechnicalName.Equals(docField.crme_CRMField, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return fields;
                        }
                    }
                }
            }
            return null;

        }
        public static completeDocumentFields GetCellField(string bookmarkValue, List<crme_docgentablefields> docTableFields, List<completeDocumentFields> completedDocumentFields)
        {
            foreach (completeDocumentFields fields in completedDocumentFields)
            {
                if (fields.templateName.Equals(bookmarkValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return fields;
                }
            }

            foreach (crme_docgentablefields docTableField in docTableFields)
            {
                if (bookmarkValue.StartsWith(docTableField.crme_name, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (completeDocumentFields fields in completedDocumentFields)
                    {
                        if (fields.templateName.Equals(docTableField.crme_CRMField, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return fields;
                        }
                    }
                }
            }
            return null;

        }
        public static List<completeDocumentFields> PopulateDynamicCellFields(Microsoft.Xrm.Sdk.AttributeCollection entityAttr, string entityName, List<crme_docgenfieldformat> fieldFormats, List<crme_docgentablefields> docTableFields)
        {
            List<completeDocumentFields> completeDocFields = new List<completeDocumentFields>();


            foreach (var mastAttr in entityAttr)
            {

                foreach (var documentField in docTableFields)
                {
                    if (mastAttr.Key == documentField.crme_CRMField)
                    {
                        completeDocumentFields thisFields = new completeDocumentFields();

                        thisFields.LogicalName = entityName;
                        thisFields.displayName = mastAttr.Key;

                        crme_docgenfieldformat thisFormat = GetThisFieldsFormat(fieldFormats, documentField.crme_FieldFormat.Id);

                        thisFields.formatId = thisFormat.Id;
                        thisFields.format = thisFormat.crme_Format;

                        switch (thisFormat.crme_DataType.Value)
                        {
                            case 935950000:
                                thisFields.dataType = "bool";
                                break;
                            case 935950001:
                                thisFields.dataType = "DateTime";
                                break;
                            case 935950002:
                                thisFields.dataType = "Decimal";
                                break;
                            case 935950003:
                                thisFields.dataType = "string";
                                break;
                            case 935950004:
                                thisFields.dataType = "Money";
                                break;
                            case 935950005:
                                thisFields.dataType = "Lookup";
                                break;
                            case 935950006:
                                thisFields.dataType = "string";
                                break;
                            case 935950007:
                                thisFields.dataType = "Decimal";
                                break;
                            case 935950008:
                                thisFields.dataType = "OptionSetValue";
                                break;
                        }

                        thisFields.actualValue = mastAttr.Value;
                        thisFields.templateName = mastAttr.Key;
                        thisFields.crmTechnicalName = mastAttr.Key;
                        thisFields.templateName = documentField.crme_name;
                        completeDocFields.Add(thisFields);
                    }
                }
            }
            return completeDocFields;

        }

        public static List<completeDocumentFields> PopulateFields(Entity entity, 
            List<crme_docgenfieldformat> fieldFormats, 
            List<crme_docgendocumentfield> docFields)
        {
            try
            {
                if (entity == null)
                    throw new Exception("entity == null");

                if (fieldFormats == null)
                    throw new Exception("fieldFormats == null");

                if (docFields == null)
                    throw new Exception("docFields == null");

                List<completeDocumentFields> fbsrFields = new List<completeDocumentFields>();

                if (entity.Attributes == null)
                    throw new Exception("entity.Attributes == null");

                foreach (var mastAttr in entity.Attributes)
                {
                    foreach (var documentField in docFields)
                    {
                        if (documentField == null)
                            throw new Exception("documentField == null");

                        if (mastAttr.Key == documentField.crme_CRMField)
                        {
                            if (documentField.crme_FieldFormat == null)
                                throw new Exception("documentField.crme_FieldFormat == null");

                            completeDocumentFields thisFields = new completeDocumentFields();

                            thisFields.LogicalName = entity.LogicalName;

                            thisFields.displayName = mastAttr.Key;
                            
                            crme_docgenfieldformat thisFormat = GetThisFieldsFormat(fieldFormats, documentField.crme_FieldFormat.Id);
                            
                            if (thisFormat == null)
                                throw new Exception("thisFormat == null");

                            thisFields.formatId = thisFormat.Id;

                            thisFields.format = thisFormat.crme_Format;

                            if(thisFormat.crme_DataType == null)
                                throw new Exception("thisFormat.crme_DataType == null " + thisFormat.crme_Format);

                            switch (thisFormat.crme_DataType.Value)
                            {
                                case 935950000:
                                    thisFields.dataType = "bool";
                                    break;
                                case 935950001:
                                    thisFields.dataType = "DateTime";
                                    break;
                                case 935950002:
                                    thisFields.dataType = "Decimal";
                                    break;
                                case 935950003:
                                    thisFields.dataType = "string";
                                    break;
                                case 935950004:
                                    thisFields.dataType = "Money";
                                    break;
                                case 935950005:
                                    thisFields.dataType = "Lookup";
                                    break;
                                case 935950006:
                                    thisFields.dataType = "string";
                                    break;
                                case 935950007:
                                    thisFields.dataType = "Decimal";
                                    break;
                                case 935950008:
                                    thisFields.dataType = "OptionSetValue";
                                    break;
                            }

                            thisFields.actualValue = mastAttr.Value;
                            thisFields.templateName = mastAttr.Key;
                            thisFields.crmTechnicalName = mastAttr.Key;
                            thisFields.templateName = documentField.crme_name;

                            fbsrFields.Add(thisFields);
                        }
                    }
                }

                return fbsrFields;
            }
            catch (Exception ex)
            {
                throw new Exception("In Populate Fields: ", ex);
            }
        }

        public static List<completeDocumentFields> PopulateTableFields(List<completeDocumentFields> completedDocumentFields, List<crme_docgentablemapping> genericTableMappings, List<crme_docgenfieldformat> fieldFormats, IOrganizationService service, Guid primaryEntityGuid, IMCSLogger logger, List<crme_docgendocumentfield> docFields)
        {


            foreach (var crmeDocgentablemapping in genericTableMappings)
            {

                //Get the fetchxml for this table.
                string fetchXml = String.Format(crmeDocgentablemapping.crme_TableQuery, primaryEntityGuid.ToString());
                EntityCollection ec = DocGen.FetchTableXml(service, fetchXml, logger);

                //Keep an index so we can look the fixed tables up by a specific row index (ex.  tr_row0, tr_row1, etc...)
                int rowIdx = 0;


                //Get the fields that are defined for this table.
                var tableFields = DocGen.GetDocGenTableFields(crmeDocgentablemapping.Id, service, logger);

                var startingRow = (crmeDocgentablemapping.crme_StartingRow != null)
                    ? crmeDocgentablemapping.crme_StartingRow.Value - 1
                    : 0;
                var endingRow = (crmeDocgentablemapping.crme_MaxNumberofRows != null)
                    ? crmeDocgentablemapping.crme_MaxNumberofRows.Value + startingRow - 1
                    : 99;
                var currentRow = 0;
                foreach (var entity in ec.Entities)
                {
                    if (currentRow >= startingRow)
                    {
                        if (currentRow <= endingRow)
                        {
                            foreach (var mastAttr in entity.Attributes)
                            {
                                //I could have a field that is a checkbox - 1 field on CRM, multiple fields on the document
                                //so I need to look through the fields on this table, driving with the CRM field.
                                foreach (var tableField in tableFields)
                                {
                                    if (mastAttr.Key == tableField.crme_CRMField)
                                    {
                                        DecodeTypeandValue(completedDocumentFields, fieldFormats, docFields, entity,
                                            mastAttr, tableField, rowIdx);
                                    }
                                }
                            }
                            rowIdx += 1;
                        }
                    }
                    currentRow += 1;
                }

            }
            return completedDocumentFields;

        }
        private static void DecodeTypeandValue(List<completeDocumentFields> completedDocumentFields, List<crme_docgenfieldformat> fieldFormats, List<crme_docgendocumentfield> docFields, Entity entity,
            KeyValuePair<string, object> mastAttr, crme_docgentablefields tableField, int rowIdx)
        {
            completeDocumentFields thisFields = new completeDocumentFields();
            thisFields.LogicalName = entity.LogicalName;
            thisFields.displayName = mastAttr.Key;
            //grab the format information
            crme_docgenfieldformat thisFormat = GetThisFieldsFormat(fieldFormats, tableField.crme_FieldFormat.Id);
            thisFields.formatId = thisFormat.Id;
            thisFields.format = thisFormat.crme_Format;
            switch (thisFormat.crme_DataType.Value)
            {
                case 935950000:
                    thisFields.dataType = "bool";
                    break;
                case 935950001:
                    thisFields.dataType = "DateTime";
                    break;
                case 935950002:
                    thisFields.dataType = "Decimal";
                    break;
                case 935950003:
                    thisFields.dataType = "string";
                    break;
                case 935950004:
                    thisFields.dataType = "Money";
                    break;
                case 935950005:
                    thisFields.dataType = "Lookup";
                    break;
                case 935950006:
                    thisFields.dataType = "string";
                    break;
                case 935950007:
                    thisFields.dataType = "Decimal";
                    break;
                case 935950008:
                    thisFields.dataType = "OptionSetValue";
                    break;
            }

            thisFields.actualValue = mastAttr.Value;
            thisFields.crmTechnicalName = mastAttr.Key;
            thisFields.templateName = tableField.crme_name + rowIdx;
            //not sure why I need this 
            var newDocFields = new crme_docgendocumentfield();
            newDocFields.crme_CRMField = mastAttr.Key;
            newDocFields.crme_name = tableField.crme_name + rowIdx;
            newDocFields.crme_FieldOutputType = tableField.crme_FieldOutputType;
            newDocFields.crme_Value = tableField.crme_Value;


            docFields.Add(newDocFields);


            thisFields.displayName = thisFields.displayName;
            completedDocumentFields.Add(thisFields);
        }
        private static crme_docgenfieldformat GetThisFieldsFormat(IEnumerable<crme_docgenfieldformat> fieldFormats, Guid fieldFormatId)
        {
            return fieldFormats.FirstOrDefault(fieldFormat => fieldFormatId == fieldFormat.Id);
        }
        public static crme_docgenfieldformat GetDefaultFormat(int dataType, List<crme_docgenfieldformat> fieldFormats)
        {
            return fieldFormats.FirstOrDefault(fieldFormat => fieldFormat.crme_DataType.Value == dataType);
        }
    }
}
