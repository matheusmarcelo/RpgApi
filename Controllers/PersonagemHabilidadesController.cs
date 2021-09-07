using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonagemHabilidadesController : ControllerBase
    {
        private readonly DataContext _context;
        public PersonagemHabilidadesController(DataContext context)
        {
            _context = context;
        }     

        [HttpPost]
        public async Task<IActionResult> AddPersonagemHabilidadeAsync(PersonagemHabilidade novoPersonagemHabilidade)
        {
            Personagem personagem = await _context.Personagens
            .Include(p => p.Arma)
            .Include(p => p.PersonagemHabilidades).ThenInclude(ps => ps.Habilidade)
            .FirstOrDefaultAsync(p => p.Id == novoPersonagemHabilidade.PersonagemId);            

            if(personagem == null)
                return BadRequest("Personagem não encontrado para o Id Informado.");
            
            Habilidade habilidade = await _context.Habilidades
                                .FirstOrDefaultAsync(s => s.Id == novoPersonagemHabilidade.HabilidadeId);

            if(habilidade == null)
                return BadRequest("Habilidade não encontrada");

            PersonagemHabilidade ph = new PersonagemHabilidade();
            ph.Personagem = personagem;
            ph.Habilidade = habilidade;

            await _context.PersonagemHabilidades.AddAsync(ph);
            await _context.SaveChangesAsync();

            return Ok(ph);            
        }

        [HttpPost("DeletePersonagemHabilidade")]
        public async Task<IActionResult> DeleteAsync(PersonagemHabilidade ph)
        {
            PersonagemHabilidade phRemover = 
            await _context.PersonagemHabilidades
                    .FirstOrDefaultAsync(phBusca => phBusca.PersonagemId == ph.PersonagemId
                                                            && phBusca.HabilidadeId == ph.HabilidadeId);

            _context.PersonagemHabilidades.Remove(phRemover);
            await _context.SaveChangesAsync();            
            
            return Ok(phRemover);
        }

        [HttpGet("{personagemId}")]
        public async Task<IActionResult> GetHabilidadesPersonagem(int personagemId)
        {
            List<PersonagemHabilidade> phLista = new List<PersonagemHabilidade>();                            
            
            phLista = await _context.PersonagemHabilidades
            .Include(p => p.Personagem)
            .Include(p => p.Habilidade)
            .Where(p => p.Personagem.Id == personagemId).ToListAsync();

            return Ok(phLista);            
        }

        [HttpGet("GetHabilidades")]
        public async Task<IActionResult> GetHabilidades()
        {
            List<Habilidade> habilidades = new List<Habilidade>();                            
            
            habilidades = await _context.Habilidades.ToListAsync();

            return Ok(habilidades);            
        }


    }
}