// using System.Net;
// using MediatR;
// using SchoolManagementApi.DTOs;
// using SchoolManagementApi.Intefaces.Profiles;
// using SchoolManagementApi.Intefaces.Uploads;

// namespace SchoolManagementApi.Commands.Uploads
// {
//   public class AddUploadDocuments
//   {
//     public class AddUploadDocumentsCommand : IRequest<GenericResponse>
//     {
//       public required string StaffId { get; set; }
//       public required List<UploadFilesDto> UploadFilesDtos { get; set; }
//     }

//     public class AddUploadDocumentsHandler(ITeachingStaffInterface teachingStaffInterface, IFileService fileService) : IRequestHandler<AddUploadDocumentsCommand, GenericResponse>
//     {
//       private readonly ITeachingStaffInterface _teachingStaffInterface = teachingStaffInterface;
//       private readonly IFileService _fileService = fileService;
//       public async Task<GenericResponse> Handle(AddUploadDocumentsCommand request, CancellationToken cancellationToken)
//       {
//         try
//         {
//           var fileNameContentList = new List<FileNameContent>();
//           foreach (var filesDto in request.UploadFilesDtos)
//           {
//             var url = await _fileService.ProcessFilesAsync(filesDto);
//             var fileNameContent = new FileNameContent
//             {
//               FileName = filesDto.FileName,
//               FileUrl = url
//             };
//             fileNameContentList.Add(fileNameContent);
//           }
//           var savedDocs = await _teachingStaffInterface.UploadDocuments(request.StaffId, fileNameContentList);
//           if (savedDocs != null)
//           {
//             return new GenericResponse
//             {
//               Status = HttpStatusCode.OK.ToString(),
//               Message = "Documents and files saved successfully",
//               Data = savedDocs
//             };
//           }
//           return new GenericResponse
//           {
//             Status = HttpStatusCode.BadRequest.ToString(),
//             Message = "Failed to save documents and files",
//           };
//         }
//         catch (Exception ex)
//         {
//           return new GenericResponse
//           {
//             Status = HttpStatusCode.InternalServerError.ToString(),
//             Message = $"An internal server error occurred - {ex.Message}"
//           };
//         }
//       }
//     }
//   }
// }