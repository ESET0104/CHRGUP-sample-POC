//using Microsoft.AspNetCore.Mvc;
//using System;

//namespace esyasoft.mobility.CHRGUP.service.api.Controllers
//{
//    [Route("api/sessions")]
//    [ApiController]
//    public class SessionController : ControllerBase
//    {
//        private readonly AppDbContext _db;
//        private readonly SessionService _sessionService;

//        public SessionController(AppDbContext db, SessionService sessionService)
//        {
//            _db = db;
//            _sessionService = sessionService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetSessions()
//        {
//            return Ok(await _db.ChargingSessions.ToListAsync());
//        }

//        [HttpPost("start")]
//        public async Task<IActionResult> StartSession([FromBody] StartSessionReq req)
//        {
//            try
//            {
//                var sessionId = await _sessionService
//                .StartSessionAsync(req.ChargerId, req.UserId);
//                return Ok(new { sessionId });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }



//        }

//        [HttpPost("stop")]
//        public async Task<IActionResult> StopSession([FromBody] StopSessionReq req)
//        {
//            try
//            {
//                await _sessionService.StopSessionAsync(req.SessionId);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }

//        }
//    }

//    public class StartSessionReq
//    {
//        public string ChargerId { get; set; }
//        public string UserId { get; set; }
//    }

//    public class StopSessionReq
//    {
//        public string SessionId { get; set; }
//    }
//}
