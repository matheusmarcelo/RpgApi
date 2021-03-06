using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RpgApi.Models;
using RpgApi.Models.Enuns;

namespace RpgApi.Controllers
{
    [ApiController]   
    [Route("[controller]")]
    public class ExercicioController : ControllerBase
    {
        private static List<Personagem> personagens = new List<Personagem>() {
            new Personagem() { Id = 1, }, //Frodo Cavaleiro             
            new Personagem() { Id = 2, Nome = "Sam", PontosVida=100, Forca=15, Defesa=25, Inteligencia=30, Classe=ClasseEnum.Cavaleiro},     
            new Personagem() { Id = 3, Nome = "Galadriel", PontosVida=100, Forca=18, Defesa=21, Inteligencia=35, Classe=ClasseEnum.Clerigo },
            new Personagem() { Id = 4, Nome = "Gandalf", PontosVida=100, Forca=18, Defesa=18, Inteligencia=37, Classe=ClasseEnum.Mago },
            new Personagem() { Id = 5, Nome = "Hobbit", PontosVida=100, Forca=20, Defesa=17, Inteligencia=31, Classe=ClasseEnum.Cavaleiro },
            new Personagem() { Id = 6, Nome = "Celeborn", PontosVida=100, Forca=21, Defesa=13, Inteligencia=34, Classe=ClasseEnum.Clerigo },
            new Personagem() { Id = 7, Nome = "Radagast", PontosVida=100, Forca=25, Defesa=11, Inteligencia=35, Classe=ClasseEnum.Mago }       
        };  

        
        //a)
        [HttpGet("GetByClasse/{classeId}")]
        public IActionResult GetByClasse(int classeId)
        {
            List<Personagem> listaBusca = personagens.FindAll(p => (int)p.Classe == classeId);
            return Ok(listaBusca);
        }

        //b)
        [HttpGet("GetByNome/{nome}")]
        public IActionResult GetbyNome(string nome)
        {
            /*Personagem p = personagens.Find(p => p.Nome == nome);

            if(p == null)            
                return NotFound("nenhum personagem com este nome foi encontrado.");
            
            return Ok(p);*/

            List<Personagem> listaFinal = personagens.FindAll(p => p.Nome == nome);

            if(listaFinal.Count == 0)            
                return NotFound("nenhum personagem com este nome foi encontrado.");

            return Ok(listaFinal);
        }

        //c
        [HttpPost("PostValidacao")]
        public IActionResult PostValidacao(Personagem novoPersonagem)
        {
            if(novoPersonagem.Defesa < 10 || novoPersonagem.Inteligencia > 30)
                return BadRequest("Defesa n??o pode ser menor que 10 ou a intelig??nca n??o pode ser maior que 30");

            personagens.Add(novoPersonagem);
            return Ok(personagens);
        }

        //d)
        [HttpPost("PostValidacaoMago")]
        public IActionResult PostValidacaoMago(Personagem novoPersonagem)
        {
            if(novoPersonagem.Classe == ClasseEnum.Mago && novoPersonagem.Inteligencia < 35)
                return BadRequest("Personagens do tipo Mago n??o podem ter intelig??ncia menor que 35.");

            personagens.Add(novoPersonagem);
            return Ok(personagens);
        }

        //e)
        [HttpGet("GetClerigoMago")]
        public IActionResult GetClerigoMago()
        {
            List<Personagem> listaSemCavaleiro = 
                personagens.FindAll(p => p.Classe != ClasseEnum.Cavaleiro)
                    .OrderByDescending(ord => ord.Inteligencia)                    
                    .ToList();

            return Ok(listaSemCavaleiro);
        }

        //f)
        [HttpGet("GetEstatisticas")]
        public IActionResult GetEstatisticas()
        {
            int quantidade = personagens.Count;
            int somaInteligencia = personagens.Sum(p => p.Inteligencia);

            //Outra maneira
            //string msg = 
                //string.Format
                    //("A lista cont??m {0} personagens e o somat??rio da intelig??ncia ?? {1}", quantidade, somaInteligencia);            
            //return Ok(msg);
            
            return Ok("A lista cont??m " + quantidade + " personagens e somat??rio da intelig??ncia ?? " + somaInteligencia);
        }





        
        

    }
}