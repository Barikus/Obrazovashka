﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Obrazovashka.Services;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificateService _certificateService;

        public CertificatesController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        [HttpGet("{userId}/course/{courseId}")]
        public async Task<IActionResult> GenerateCertificate(int userId, int courseId)
        {
            var result = await _certificateService.GenerateCertificateAsync(userId, courseId);
            if (result.Success)
                return Ok(result);

            return NotFound();
        }
    }
}