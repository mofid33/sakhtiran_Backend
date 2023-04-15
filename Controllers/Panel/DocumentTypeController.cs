using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class DocumentTypeController : ControllerBase
    {
        public IDocumentTypeService _documentTypeService { get; }
        public DocumentTypeController(IDocumentTypeService documentTypeService)
        {
            this._documentTypeService = documentTypeService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<DocumentTypeDto>))]
        public async Task<IActionResult> Add([FromBody] DocumentTypeDto documentType)
        {
            var result = await _documentTypeService.DocumentTypeAdd(documentType);
            return new Response<DocumentTypeDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<DocumentTypeDto>))]
        public async Task<IActionResult> Edit([FromBody] DocumentTypeDto documentType)
        {
            var result = await _documentTypeService.DocumentTypeEdit(documentType);
            return new Response<DocumentTypeDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _documentTypeService.DocumentTypeDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<DocumentTypeGetDto>>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _documentTypeService.DocumentTypeGetAll(pagination);
            return new Response<Pagination<DocumentTypeGetDto>>().ResponseSending(result);
        }

        [HttpGet("{documentTypeId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<DocumentTypeGetDto>))]
        public async Task<IActionResult> GetDocumentTypeById([FromRoute] int documentTypeId)
        {
            var result = await _documentTypeService.GetDocumentTypeById(documentTypeId);
            return new Response<DocumentTypeGetDto>().ResponseSending(result);
        }

        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAccept([FromBody] AcceptDto accept)
        {
            var result = await _documentTypeService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }

    }
}