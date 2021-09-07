using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]   
    [Route("[controller]")] 
    public class ArmasController : ControllerBase
    {        
        private readonly DataContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArmasController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int ObterUsuarioId()
        {            
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }       

        [HttpGet]                
        public async Task<IActionResult> GetAsync()
        {            
            List<Arma> armas = await _context.Armas
            .Include(p => p.Personagem)
            .ToListAsync();
            return Ok(armas);
        } 

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleAsync(int id)
        {

            Arma a = await _context.Armas
            .Include(p => p.Personagem)
            .FirstOrDefaultAsync(a => a.Id == id);
            return Ok(a);
        }

        [HttpPost]
        public async Task<IActionResult> AddArmaAsync(Arma novaArma)
        {
            Personagem personagem = await _context.Personagens
                .FirstOrDefaultAsync(p => p.Id == novaArma.PersonagemId 
                    && p.Usuario.Id == ObterUsuarioId());

            if(personagem == null)
                return BadRequest("Seu usuário não contém personagens com o Id do Personagem informado.");

            Arma buscaArma = await _context.Armas
                .FirstOrDefaultAsync(a => a.PersonagemId == novaArma.PersonagemId);

            if(buscaArma != null)
                return BadRequest("O Personagem selecionado já contém uma arma atribuída a ele.");
            
            await _context.Armas.AddAsync(novaArma);        
            await _context.SaveChangesAsync();
            
            List<Arma> armas = await _context.Armas.Where(a => a.PersonagemId == novaArma.PersonagemId).ToListAsync();            
            
            return Ok(armas);
        }

       










       













        /*[HttpPost]
        public async Task<IActionResult> AddArmaAsync(Arma novaArma)
        {            
            Personagem personagem = await _context.Personagens
                .FirstOrDefaultAsync(p => p.Id == novaArma.PersonagemId);

            if(personagem == null)
                return BadRequest("Não existe personagem com o id informado.");

            await _context.Armas.AddAsync(novaArma);        
            await _context.SaveChangesAsync();
            
            List<Arma> armas = await _context.Armas
            .Where(p => p.PersonagemId == novaArma.PersonagemId)
            .ToListAsync();            
            
            return Ok(armas);
        }*/

        [HttpPut]
        public async Task<IActionResult> UpdateArmaAsync(Arma a)
        {
             _context.Armas.Update(a);
            await _context.SaveChangesAsync();
            
            return Ok(a);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {   
            Arma aRemover = _context.Armas.FirstOrDefault(a => a.Id == id);            
            
            _context.Armas.Remove(aRemover);            
            _context.SaveChanges();            
           
            List<Arma> armas =_context.Armas.ToList(); 
            
            return Ok(armas);
        }
    }
}