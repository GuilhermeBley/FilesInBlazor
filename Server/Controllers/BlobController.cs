using AutoMapper;
using FilesInBlazor.Server.Services;
using FilesInBlazor.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FilesInBlazor.Server.Controllers;

[ApiController, Route("api/[controller]")]
public class BlobController : ControllerBase
{
    private readonly ILogger<BlobController> _logger;
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;

    public BlobController(ILogger<BlobController> logger, IBlobService blobService, IMapper mapper)
    {
        _logger = logger;
        _blobService = blobService;
        _mapper = mapper;
    }

    [HttpGet, Route("all")]
    public async Task<ActionResult<IEnumerable<BlobInfoDto>>> GetAll()
    {
        try
        {
            var results = await _blobService.GetBlobItems();

            return Ok(_mapper.Map<IEnumerable<BlobInfoDto>>(results));
        }
        catch (Exception)
        {
            return BadRequest("Falha em coleta de blobs.");
        }
    }

    [HttpGet, Route("uri")]
    public async Task<ActionResult<string>> GetUri(string fileName)
    {
        try
        {
            var result = await _blobService.GetUriBlobOrDefault(fileName);

            if (result is null)
                return NotFound();

            return Ok(result.AbsoluteUri);
        }
        catch (Exception)
        {
            return BadRequest("Falha em coleta de blobs.");
        }
    }

    [HttpPost, Consumes("multipart/form-data")]
    public async Task<ActionResult<FileUploadResponses>> Post([FromForm]IEnumerable<IFormFile> formFiles)
    {
        List<FileUploadResponse> fileResponseList = new();

        foreach (IFormFile formFile in formFiles)
        {
            FileUploadResponse fileResponse = new()
            {
                FileName = formFile.FileName,
                Size = formFile.Length
            };

            if (await _blobService.ExistsFile(formFile.FileName))
            {
                fileResponse.Status = "Arquivo já existe.";
            }
            else if (formFile.Length > 0)
            {
                try
                {
                    using (var stream = formFile.OpenReadStream())
                    {
                        await _blobService.UploadFile(formFile.FileName, stream);
                    }

                    fileResponse.Status = "Ok";
                }
                catch
                {
                    fileResponse.Status = "Falha em upload.";
                }
            }
            else
            {
                fileResponse.Status = "Arquivo não possuí conteudo.";
            }

            fileResponseList.Add(fileResponse);
        }

        if (!fileResponseList.Any())
            return BadRequest("Nenhum arquivo adicionado.");

        return Ok(new FileUploadResponses { Files= fileResponseList });
    }

    [HttpDelete]
    public async Task<ActionResult> Delete([FromQuery] string fileName)
    {
        return Ok();   
    }
}