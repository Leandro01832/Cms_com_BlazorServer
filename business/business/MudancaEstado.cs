using business.business.Contrato;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class MudancaEstado : BaseModel, IMudancaEstado
    {
        [Key, ForeignKey("ChangeContent")]
        public new Int64? Id { get; set; }
        public int Pontos { get; set; }
        public long Curtidas { get; set; }
        public long Compartilhamentos { get; set; }
        public string? Type { get; set; }
        public long IdContent { get; set; }
        public virtual ChangeContent? ChangeContent { get; set; }

        public Pagina MudarEstado( UserContent m, long curtidas, long compartilhamentos)
        {
            setarPropriedades(m, curtidas, compartilhamentos, "UserContent");

            Pagina pagina = new Pagina(m.Story)
            {
                Html = m.Html,
                Produto = m.Produto,
                Rotas = m.Rotas,
                  Titulo = m.Titulo,
                   mudanca = this,
                    Comentario = m.Comentario,
                     
            };

            return pagina;
        }

        public Pagina MudarEstado2(Comment m, long curtidas, long compartilhamentos)
        {
            setarPropriedades(m, curtidas, compartilhamentos, "Comment");

            Pagina pagina = new Pagina(m.Story)
            {
                Html = m.Html,
                Produto = m.Produto,
                Rotas = m.Rotas,
                Titulo = m.Titulo,
                mudanca = this,
                Comentario = m.Comentario,

            };

            return pagina;
        }

        private void setarPropriedades(Content m, long curtidas, long compartilhamentos, string tipo)
        {
            this.Curtidas = curtidas;
            this.Compartilhamentos = compartilhamentos;
            this.Type = tipo;
            this.IdContent = m.Id;

            if (curtidas > 100000 && curtidas < 200000) this.Pontos += 100;
            if (curtidas > 200000 && curtidas < 300000) this.Pontos += 200;
            if (curtidas > 300000 && curtidas < 400000) this.Pontos += 300;
            if (curtidas > 400000 && curtidas < 500000) this.Pontos += 400;
            if (curtidas > 500000 && curtidas < 600000) this.Pontos += 500;
            if (curtidas > 600000 && curtidas < 700000) this.Pontos += 600;
            if (curtidas > 700000 && curtidas < 800000) this.Pontos += 700;
            if (curtidas > 800000 && curtidas < 900000) this.Pontos += 800;
            if (curtidas > 900000) this.Pontos += 900;

            if (compartilhamentos > 100000 && compartilhamentos < 200000) this.Pontos += 100;
            if (compartilhamentos > 200000 && compartilhamentos < 300000) this.Pontos += 200;
            if (compartilhamentos > 300000 && compartilhamentos < 400000) this.Pontos += 300;
            if (compartilhamentos > 400000 && compartilhamentos < 500000) this.Pontos += 400;
            if (compartilhamentos > 500000 && compartilhamentos < 600000) this.Pontos += 500;
            if (compartilhamentos > 600000 && compartilhamentos < 700000) this.Pontos += 600;
            if (compartilhamentos > 700000 && compartilhamentos < 800000) this.Pontos += 700;
            if (compartilhamentos > 800000 && compartilhamentos < 900000) this.Pontos += 800;
            if (compartilhamentos > 900000) this.Pontos += 900;
        }
    }
}
