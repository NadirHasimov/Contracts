using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AdyContracts.Models;
using AdyContracts.DALC;
using AdyContracts.DomainModels;
using AdyContracts.Utils;
using System.Drawing;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System.Xml.Linq;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Globalization;
using System.Web;
using System.Web.Security;

namespace AdyContracts.Controllers
{
    [Authorize]
    [Log]
    public class HomeController : Controller
    {
        [AuthorizeUsersRoles]
        public ActionResult Index(DocumentFilterModel documentFilterModel)
        {
            ViewBag.DocTypes = DocumentDALC.GetTypesDueToQuery(SqlQueries.Document.getTypes);
            ViewBag.ReceivingAuthorities = DocumentDALC.GetTypesDueToQuery(SqlQueries.Document.getReceivingAuthorities);
            ViewBag.ClassTypes = DocumentDALC.GetTypesDueToQuery(SqlQueries.Document.getClassTypes);
            UserDALC.addLog(HttpContext, true, "GET");
            return View();
        }

        [AuthorizeUsersRoles]
        public ActionResult Upload(int until2018 = 2)
        {
            ViewBag.DocTypes = DocumentDALC.GetTypesDueToQuery(SqlQueries.Document.getTypes);
            ViewBag.ReceivingAuthorities = DocumentDALC.GetTypesDueToQuery(SqlQueries.Document.getReceivingAuthorities);
            ViewBag.ClassTypes = DocumentDALC.GetTypesDueToQuery(SqlQueries.Document.getClassTypes);
            ViewBag.SelectListItems = DocumentDALC.GetSelectLists(3);
            UserDALC.addLog(HttpContext, true, "GET");
            return View();
        }

        [HttpPost]
        [AuthorizeUsersRoles]
        public ActionResult Upload(DocumentViewModel model)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            PdfDocument pdfDocument;
            bool status = false;
            string result = "", description = "";
            try
            {
                string extension = System.IO.Path.GetExtension(model.file.FileName);
                int receivingId;
                if (model.file.ContentLength > 0 && string.Equals(extension, ".pdf"))
                {
                    pdfDocument = new PdfDocument(model.file.InputStream);
                    pdfDocument.FileInfo.IncrementalUpdate = false;
                    foreach (PdfPageBase page in pdfDocument.Pages)
                    {
                        Image[] images = page.ExtractImages();
                        if (images != null && images.Length > 0)
                        {
                            for (int j = 0; j < images.Length; j++)
                            {
                                Image image = images[j];
                                PdfBitmap bp = new PdfBitmap(image);
                                bp.Quality = 20;
                                page.ReplaceImage(j, bp);
                            }
                        }
                    }
                    string _fileName = model.file.FileName;
                    string _path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), model.id + ".pdf");
                    AttachmentModel attachmentModel = new AttachmentModel
                    {
                        fileName = _fileName,
                        filePath = _path,
                        conntentType = model.file.ContentType,
                        FileSize = model.file.ContentLength
                    };
                    if (!int.TryParse(model.ReceivingAuthorityId, out receivingId))
                    {
                        model.ReceivingAuthorityId = DocumentDALC.
                                                                CreateNewReceiver(model.ReceivingAuthorityId).ToString();
                        if (int.Parse(model.ReceivingAuthorityId) == -1)
                        {
                            return new RedirectResult(Url.Action("Upload", "Home") + "#error");
                        }
                    }
                    DocumentModel documentModel = new DocumentModel
                    {
                        docNumber = model.id,
                        typeId = model.type,
                        description = model.description,
                        govRegNumber = model.govRegNumber,
                        receivingAuthorityId = int.Parse(model.ReceivingAuthorityId),
                        registrationDate = model.registrationDate,
                        attachmentModel = attachmentModel,
                        terminatedDocNumber = model.terminationContract
                    };
                    pdfDocument.SaveToFile(_path);
                    result = DocumentDALC.InsertIntoDb(documentModel, extension) ? "#success" : "#error";/*------------------->Exception handler<------------------------------*/
                }
                return new RedirectResult(Url.Action("Upload", "Home") + result);
            }
            catch (Exception exc)
            {
                description = exc.Message.ToString();
                return new RedirectResult(Url.Action("Upload", "Home") + "#error");
            }
            finally
            {
                status = string.Equals(result, "#success");
                UserDALC.addLog(HttpContext, status, description);
            }
        }

        [HttpPost]
        public ActionResult UploadWordDocument(DocumentViewModel documentViewModel)
        {
            string extension = System.IO.Path.GetExtension(documentViewModel.file.FileName);
            int receivingId;
            string result = "";
            if (String.Equals(extension, ".doc") || String.Equals(extension, ".docx"))
            {
                string _fileName = documentViewModel.file.FileName;
                string _path = String.Equals(extension, ".doc") ?
                    System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), documentViewModel.id + ".doc") :
                    System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), documentViewModel.id + ".docx");
                string wordText = "";
                using (WordprocessingDocument document = WordprocessingDocument.Open(documentViewModel.file.InputStream, false))
                {
                    wordText = document.MainDocumentPart.Document.Body.InnerText;
                }

                if (!int.TryParse(documentViewModel.ReceivingAuthorityId, out receivingId))
                {
                    documentViewModel.ReceivingAuthorityId = DocumentDALC.
                                   CreateNewReceiver(documentViewModel.ReceivingAuthorityId).ToString();
                    if (int.Parse(documentViewModel.ReceivingAuthorityId) == -1)
                    {
                        return new RedirectResult(Url.Action("Upload", "Home") + "#error");
                    }
                }
                AttachmentModel attachmentModel = new AttachmentModel
                {
                    fileName = _fileName,
                    filePath = _path,
                    conntentType = documentViewModel.file.ContentType,
                    FileSize = documentViewModel.file.ContentLength
                };
                DocumentModel documentModel = new DocumentModel
                {
                    docNumber = documentViewModel.id,
                    typeId = documentViewModel.type,
                    description = documentViewModel.description,
                    govRegNumber = documentViewModel.govRegNumber,
                    receivingAuthorityId = int.Parse(documentViewModel.ReceivingAuthorityId),
                    registrationDate = documentViewModel.registrationDate,
                    attachmentModel = attachmentModel
                };
                result = DocumentDALC.InsertIntoDb(documentModel, extension, wordText) ? "#success" : "#error";/*------------------->Exception handler<------------------------------*/
                documentViewModel.file.SaveAs(_path);
            }
            if (documentViewModel.pdfFile != null && String.Equals(System.IO.Path.GetExtension(documentViewModel.pdfFile.FileName), ".pdf"))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                string path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), documentViewModel.id + ".pdf");
                PdfDocument pdfDocument = new PdfDocument(documentViewModel.pdfFile.InputStream);
                pdfDocument.FileInfo.IncrementalUpdate = false;
                foreach (PdfPageBase page in pdfDocument.Pages)
                {
                    Image[] images = page.ExtractImages();
                    if (images != null && images.Length > 0)
                    {
                        for (int j = 0; j < images.Length; j++)
                        {
                            Image image = images[j];
                            PdfBitmap bp = new PdfBitmap(image);
                            bp.Quality = 20;
                            page.ReplaceImage(j, bp);
                        }
                    }
                }
                documentViewModel.pdfFile.SaveAs(path);
            }
            return new RedirectResult(Url.Action("Upload", "Home") + result);
        }

        [HttpPost]
        public ActionResult UploadOrderDocument(DocumentViewModel documentViewModel)
        {
            string extension = System.IO.Path.GetExtension(documentViewModel.file.FileName);
            int receivingId;
            string result = "";
            if (String.Equals(extension, ".doc") || String.Equals(extension, ".docx"))
            {
                string _fileName = documentViewModel.file.FileName;
                string _path = String.Equals(extension, ".doc") ?
                    System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), documentViewModel.id + ".doc") :
                    System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), documentViewModel.id + ".docx");
                string wordText = "";
                documentViewModel.file.SaveAs(_path);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(_path, true))
                {
                    wordText = doc.MainDocumentPart.Document.Body.InnerText;
                }
                if (!int.TryParse(documentViewModel.ReceivingAuthorityId, out receivingId) && !String.IsNullOrEmpty(documentViewModel.ReceivingAuthorityId))
                {
                    documentViewModel.ReceivingAuthorityId = DocumentDALC.
                                   CreateNewReceiver(documentViewModel.ReceivingAuthorityId).ToString();
                    if (int.Parse(documentViewModel.ReceivingAuthorityId) == -1)
                    {
                        return new RedirectResult(Url.Action("Upload", "Home") + "#error");
                    }
                }
                AttachmentModel attachmentModel = new AttachmentModel
                {
                    fileName = _fileName,
                    filePath = _path,
                    conntentType = documentViewModel.file.ContentType,
                    FileSize = documentViewModel.file.ContentLength
                };
                documentViewModel.attachmentViewModel = MapToAttachmentViewModel(attachmentModel);
                result = DocumentDALC.InsertOrderDocument(documentViewModel, extension, wordText) ? "#success" : "#error";/*------------------->Exception handler<------------------------------*/
            }
            if (documentViewModel.paragraphs != null)
            {
                DocumentDALC.TerminateOrders(documentViewModel.paragraphs);
            }
            if (documentViewModel.changedOrderDocs != null)
            {
                DocumentDALC.InsertChangedOrders(documentViewModel.id, documentViewModel.changedOrderDocs);
            }
            if (documentViewModel.pdfFile != null && String.Equals(System.IO.Path.GetExtension(documentViewModel.pdfFile.FileName), ".pdf"))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                string path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), documentViewModel.id + ".pdf");
                PdfDocument pdfDocument = new PdfDocument(documentViewModel.pdfFile.InputStream);
                pdfDocument.FileInfo.IncrementalUpdate = false;
                foreach (PdfPageBase page in pdfDocument.Pages)
                {
                    Image[] images = page.ExtractImages();
                    if (images != null && images.Length > 0)
                    {
                        for (int j = 0; j < images.Length; j++)
                        {
                            Image image = images[j];
                            PdfBitmap bp = new PdfBitmap(image);
                            bp.Quality = 20;
                            page.ReplaceImage(j, bp);
                        }
                    }
                }
                documentViewModel.pdfFile.SaveAs(path);
            }
            return new RedirectResult(Url.Action("Upload", "Home") + result);
        }

        [HttpPost]
        public ActionResult FilterDocuments(DocumentFilterModel filterModel)
        {
            List<AttachmentModel> filteredDocuments = DocumentDALC.Filter(filterModel);
            return Json(filteredDocuments, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAttachments()
        {
            int count = 0;
            string message = "Gmail hesabında yeni fayl yoxdur!";
            bool result = true;
            try
            {
                List<AttachmentModel> attachments = AttachmentDALC.getAttachmentsFroMail(Server.MapPath("~/UploadedFiles"));
                count = attachments.Count;
                if (count != 0)
                {
                    AttachmentDALC.AddAttachmentInfoIntoDb(attachments);
                    message = count + " fayl bazaya əlavə olundu.";
                }
                return Json(new { result, message, count });
            }
            catch (Exception ex)
            {
                result = false;
                message = "Xəta baş verdi! Zəhmət olmasa yenidən cəhd edin! Error message: " + ex.ToString();
                return Json(new { result, message });
            }
        }

        public ActionResult AjaxHandler(jQueryDataTableParamModel param)
        {
            int count = AttachmentDALC.Count();
            int filteredCount = count;
            var sortColumnIndex = int.Parse(Request["iSortCol_0"] ?? "1");
            var sortDirction = Request["sSortDir_0"];
            var idFilter = Request["sSearch_0"].ToString();
            var idFilterAvailable = !String.IsNullOrEmpty(idFilter);
            var typeFilter = Request["sSearch_1"].ToString();
            var typeFilterAvailable = !String.IsNullOrEmpty(typeFilter);
            var effectiveDateFilter = Request["sSearch_2"].ToString();
            var dateFilterAvailable = !String.IsNullOrEmpty(effectiveDateFilter);
            var descriptionFilter = Request["sSearch_3"].ToString();
            var descriptionFilterAvailable = !String.IsNullOrEmpty(descriptionFilter);
            var filterAvailable = dateFilterAvailable || idFilterAvailable || typeFilterAvailable || descriptionFilterAvailable;
            Func<AttachmentModel, string> orderingFunction = (c => sortColumnIndex == 0 ? c.id :
                                                                  sortColumnIndex == 1 ? c.contractType :
                                                   sortColumnIndex == 2 ? c.effectiveDate.ToShortDateString() : c.description);
            List<AttachmentModel> attachments = new List<AttachmentModel>();
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                attachments = AttachmentDALC.getAtttachmentsFromDb();

                if (sortDirction == "asc")
                {
                    attachments = attachments.OrderBy(orderingFunction).ToList();
                }
                else
                {
                    attachments = attachments.OrderByDescending(orderingFunction).ToList();
                }

                filteredCount = attachments.Count;
                attachments = attachments.Skip(param.iDisplayStart).Take(param.iDisplayLength == -1 ? count : param.iDisplayLength).ToList();
            }
            else if (filterAvailable)
            {
                attachments = AttachmentDALC.getAtttachmentsFromDb().Where(
                   m => (m.contractType.Contains(typeFilter) && typeFilterAvailable)
                   ||
                   (m.id.Contains(idFilter) && idFilterAvailable)
                   ||
                   (m.description.Contains(descriptionFilter) && descriptionFilterAvailable)
                   ||
                   (m.effectiveDate.ToString().Contains(effectiveDateFilter) && dateFilterAvailable)).ToList();
                if (sortDirction == "asc")
                {
                    attachments = attachments.OrderBy(orderingFunction).ToList();
                }
                else
                {
                    attachments = attachments.OrderByDescending(orderingFunction).ToList();
                }
                filteredCount = attachments.Count;
                attachments = attachments.Skip(param.iDisplayStart).Take(param.iDisplayLength == -1 ? count : param.iDisplayLength).ToList();
            }
            else
            {
                attachments = AttachmentDALC.getAtttachmentsFromDb();
                if (sortDirction == "asc")
                {
                    attachments = attachments.OrderBy(orderingFunction).ToList();
                }
                else
                {
                    attachments = attachments.OrderByDescending(orderingFunction).ToList();
                }
                attachments = attachments.Skip(param.iDisplayStart).Take(param.iDisplayLength == -1 ? count : param.iDisplayLength).ToList();
            }


            var data = from c in attachments
                       select new[] { c.id.Trim(), c.contractType, c.effectiveDate.Date.ToShortDateString(), c.description, c.opreationsColumn };


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = filteredCount,
                iTotalDisplayRecords = filteredCount,
                aaData = data
            },
            JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeUsersRoles]
        public ActionResult DeleteDocument(string id)
        {
            bool result = DocumentDALC.Delete(id);
            string message = result ? "Seçdiyiniz sənəd silindi!" : "Xəta baş verdi! Yenidən cəhd edin!";
            return Json(new { result, message });
        }

        [AuthorizeUsersRoles]
        public ActionResult Edit(string id, int typeId)
        {
            ViewBag.DocTypes = DocumentDALC.GetTypesDueToQuery(SqlQueries.Document.getTypes);
            ViewBag.ReceivingAuthorities = DocumentDALC.GetTypesDueToQuery(SqlQueries.Document.getReceivingAuthorities);
            DocumentViewModel documentViewModel = MapToDocumentViewModel(DocumentDALC.GetDocumentById(id, typeId));
            ViewBag.SelectListItems = DocumentDALC.GetSelectLists(typeId);
            HttpContext.Response.Headers.Set("Cache-Control", "private, max-age=0");
            return View(documentViewModel);
        }

        [HttpPost]
        [AuthorizeUsersRoles]
        public ActionResult Edit(DocumentViewModel documentViewModel, string orginalId)
        {
            AttachmentModel attachmentModel = new AttachmentModel();
            int receivingId;
            if (!int.TryParse(documentViewModel.ReceivingAuthorityId, out receivingId))
            {
                documentViewModel.ReceivingAuthorityId = DocumentDALC.
                                                        CreateNewReceiver(documentViewModel.ReceivingAuthorityId).ToString();
                if (int.Parse(documentViewModel.ReceivingAuthorityId) == -1)
                {
                    return new RedirectResult(Url.Action("Upload", "Home") + "#error");
                }
            }
            DocumentModel documentModel = MapToDocumentModel(documentViewModel);
            documentModel.attachmentModel = attachmentModel;
            if (DocumentDALC.Edit(documentModel, orginalId))
            {
                if (documentViewModel.file != null)
                {
                    string _path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles"), orginalId + System.IO.Path.GetExtension(documentViewModel.file.FileName));
                    documentViewModel.file.SaveAs(_path);
                }
                return new RedirectResult(Url.Action("Upload", "Home") + "#success");
            }
            return new RedirectResult(Url.Action("Upload", "Home") + "#error");
        }

        [NoCache]
        public ActionResult ViewFile(string fileName, string typeId)
        {
            string docNumber = System.IO.Path.GetFileNameWithoutExtension(fileName);
            DocumentViewModel viewModel = MapToDocumentViewModel(DocumentDALC.GetDocumentsForDescription(docNumber, typeId));
            if (fileName.Contains(".doc"))
            {
                byte[] byteArray = System.IO.File.ReadAllBytes(Server.MapPath("~/UploadedFiles/" + fileName));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(byteArray, 0, byteArray.Length);
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                    {
                        HtmlConverterSettings settings = new HtmlConverterSettings()
                        {
                            PageTitle = "Sənəd"
                        };
                        XElement html = OpenXmlPowerTools.HtmlConverter.ConvertToHtml(doc, settings);
                        ViewBag.HtmlPage = html.ToStringNewLineOnAttributes();
                    }
                }
            }
            ViewBag.FilePath = "UploadedFiles/" + fileName;
            return View(viewModel);
        }

        public ActionResult ViewPdf(string fileName)
        {
            ViewBag.FilePath = "UploadedFiles/" + fileName + ".pdf";
            return View();
        }

        public ActionResult CheckDocNumberExists(string id)
        {
            if (!DocumentDALC.CheckDocNumber(id))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void DownloadWord(string fileName)
        {
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            Response.TransmitFile(Server.MapPath("~/UploadedFiles/" + fileName));
            Response.End();
        }

        public JsonResult GetOrderDocLists(int typeId)
        {
            var list = DocumentDALC.GetOrderDocumentList(typeId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParagraphs(string docNumber)
        {
            var list = DocumentDALC.GetParagraphs(docNumber);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _attachmentList()
        {
            var model = AttachmentDALC.getAtttachmentsFromDb(0);
            return PartialView(model);
        }

        private AttachmentViewModel MapToAttachmentViewModel(AttachmentModel model)
        {
            return new AttachmentViewModel
            {
                fileName = model.fileName,
                id = model.id,
                conntentType = model.conntentType,
                filePath = model.filePath,
                description = model.description,
                FileSize = model.FileSize,
                contractType = model.contractType,
                effectiveDate = model.effectiveDate,
                typeId = model.typeId
            };
        }

        private DocumentModel MapToDocumentModel(DocumentViewModel viewModel)
        {
            return new DocumentModel
            {
                description = viewModel.description,
                docNumber = viewModel.id,
                govRegNumber = viewModel.govRegNumber,
                receivingAuthorityId = int.Parse(viewModel.ReceivingAuthorityId),
                registrationDate = viewModel.registrationDate,
                effectiveDate = viewModel.effectiveDate,
                typeId = viewModel.type,
                terminatedDocNumber = viewModel.terminatedDocNumber,
                rbtnTerminationType = viewModel.rbtnTerminationType,
                changedOrders = viewModel.changedOrderDocs
            };
        }

        private DocumentViewModel MapToDocumentViewModel(DocumentModel documentModel)
        {
            return new DocumentViewModel
            {
                fileName = documentModel.fileName,
                description = documentModel.description,
                id = documentModel.docNumber,
                govRegNumber = documentModel.govRegNumber,
                ReceivingAuthorityId = documentModel.receivingAuthorityId.ToString(),
                registrationDate = documentModel.registrationDate,
                type = documentModel.typeId,
                templateParameters = documentModel.templateParameters,
                terminatedDocNumber = documentModel.terminatedDocNumber,
                effectiveDate = documentModel.effectiveDate != null ? documentModel.effectiveDate : DateTime.Parse("06.09.1966"),
                changedOrderDocs = documentModel.changedOrders,
                rbtnTerminationType = documentModel.rbtnTerminationType,
                receiver = documentModel.receiver,
                docType = documentModel.type,
                status = documentModel.status
            };
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            UserDALC.addLog(HttpContext, false, filterContext.Exception.Message);
            filterContext.ExceptionHandled = true;
            base.OnException(filterContext);
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                if (userLang != "")
                {
                    lang = userLang;
                }
                else
                {
                    lang = LanguageManager.GetDefaultLanguage();
                }
            }
            new LanguageManager().SetLanguage(lang);
            return base.BeginExecuteCore(callback, state);
        }
    }
}