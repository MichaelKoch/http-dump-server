using http.dump.service.models;
using http.dump.service.repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace http.dump.service.Controllers
{
    [Route("$api/result")]
    [ApiController]
    public class DumpController : ControllerBase
    {

        private DumpRepository _repo;
        public DumpController(DumpRepository repo)
        {
            _repo = repo;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public DumpModel? Get(string id)
        {
            return _repo.Read(id);
        }
        // GET api/<ValuesController>/5
        [HttpGet("")]
        public IList<string> GetAll()
        {
            return _repo.getIds();
        }
    }
}
