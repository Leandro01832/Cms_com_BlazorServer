
using System.Linq;
using System;
using System.Collections.Generic;
using business.Group;
using System.Collections;
using business;

namespace Models
{
    public class ClassArray
    {

        public int[] RetornarArray(Story story, int comparacao, long filtro, int Indice, int IndiceSubStory, int? IndiceGrupo = null,
          int? IndiceSubGrupo = null, int? IndiceSubSubGrupo = null, int? IndiceCamadaSeis = null, int? IndiceCamadaSete = null,
          int? IndiceCamadaOito = null, int? IndiceCamadaNove = null, int? IndiceCamadaDez = null)
        {
            int[]? result = null;
            bool condicao = false;
            long num;

            if (IndiceCamadaDez != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}{IndiceSubGrupo}{IndiceSubSubGrupo}{IndiceCamadaSeis}{IndiceCamadaSete}{IndiceCamadaOito}{IndiceCamadaNove}{IndiceCamadaDez}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    foreach (var item2 in story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                    {
                        foreach (var item3 in story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                        {
                            foreach (var item4 in story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                            {
                                foreach (var item5 in story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                {
                                    foreach (var item6 in story.Filtro!.Where(str => str is CamadaSete && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                    {
                                        foreach (var item7 in story.Filtro!.Where(str => str is CamadaOito && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                        {
                                            foreach (var item8 in story.Filtro!.Where(str => str is CamadaNove && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                            {
                                                foreach (var item9 in story.Filtro!.Where(str => str is CamadaDez && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                                {
                                                    var lista1 = (List<SubStory   >)revertLista(story.Filtro!.Where(str => str is SubStory    && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var lista2 = (List<Grupo      >)revertLista(story.Filtro!.Where(str => str is Grupo       && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var lista3 = (List<SubGrupo   >)revertLista(story.Filtro!.Where(str => str is SubGrupo    && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var lista4 = (List<SubSubGrupo>)revertLista(story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var lista5 = (List<CamadaSeis >)revertLista(story.Filtro!.Where(str => str is CamadaSeis  && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var lista6 = (List<CamadaSete >)revertLista(story.Filtro!.Where(str => str is CamadaSete  && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var lista7 = (List<CamadaOito >)revertLista(story.Filtro!.Where(str => str is CamadaOito  && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var lista8 = (List<CamadaNove >)revertLista(story.Filtro!.Where(str => str is CamadaNove  && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var lista9 = (List<CamadaDez  >)revertLista(story.Filtro!.Where(str => str is CamadaDez   && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                    var var1 = lista1.IndexOf((SubStory   )item) + 1;
                                                    var var2 = lista2.IndexOf((Grupo      )item2) + 1;
                                                    var var3 = lista3.IndexOf((SubGrupo   )item3) + 1;
                                                    var var4 = lista4.IndexOf((SubSubGrupo)item4) + 1;
                                                    var var5 = lista5.IndexOf((CamadaSeis )item5) + 1;
                                                    var var6 = lista6.IndexOf((CamadaSete )item6) + 1;
                                                    var var7 = lista7.IndexOf((CamadaOito )item7) + 1;
                                                    var var8 = lista8.IndexOf((CamadaNove )item8) + 1;
                                                    var var9 = lista9.IndexOf((CamadaDez  )item9) + 1;
                                                    long num2 = long.Parse($"{Indice}{var1}{var2}{var3}{var4}{var5}{var6}{var7}{var8}{var9}");
                                                    if (comparacao == 1)
                                                    {
                                                        if (num2 > num)
                                                        {
                                                            condicao = true;
                                                            result = new int[5];
                                                            result[0] = Indice;
                                                            result[1] = var1;
                                                            result[2] = var2;
                                                            result[3] = var3;
                                                            result[4] = var4;
                                                            result[5] = var5;
                                                            result[6] = var6;
                                                            result[7] = var7;
                                                            result[8] = var8;
                                                            result[9] = var9;
                                                            break;
                                                        }
                                                    }
                                                    else if (comparacao == 2)
                                                    {
                                                            lista1.Reverse();
                                                            lista2.Reverse();
                                                            lista3.Reverse();
                                                            lista4.Reverse();
                                                            lista5.Reverse();
                                                            lista6.Reverse();
                                                            lista7.Reverse();
                                                            lista8.Reverse();
                                                            lista9.Reverse();
                                                        if (num2 < num)
                                                        {
                                                            condicao = true;
                                                            result = new int[5];
                                                            result[0] = Indice;
                                                            result[1] = var1;
                                                            result[2] = var2;
                                                            result[3] = var3;
                                                            result[4] = var4;
                                                            result[5] = var5;
                                                            result[6] = var6;
                                                            result[7] = var7;
                                                            result[8] = var8;
                                                            result[9] = var9;
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (item9.Id == filtro)
                                                        {
                                                            condicao = true;
                                                            result = new int[5];
                                                            result[0] = Indice;
                                                            result[1] = var1;
                                                            result[2] = var2;
                                                            result[3] = var3;
                                                            result[4] = var4;
                                                            result[5] = var5;
                                                            result[6] = var6;
                                                            result[7] = var7;
                                                            result[8] = var8;
                                                            result[9] = var9;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (condicao) break;

                                    }
                                    if (condicao) break;
                                }
                                if (condicao) break;

                            }
                            if (condicao) break;

                        }
                        if (condicao) break;

                    }
                    if (condicao) break;
                }
            }
            else
            if (IndiceCamadaNove != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}{IndiceSubGrupo}{IndiceSubSubGrupo}{IndiceCamadaSeis}{IndiceCamadaSete}{IndiceCamadaOito}{IndiceCamadaNove}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    foreach (var item2 in story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                    {
                        foreach (var item3 in story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                        {
                            foreach (var item4 in story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                            {
                                foreach (var item5 in story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                {
                                    foreach (var item6 in story.Filtro!.Where(str => str is CamadaSete && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                    {
                                        foreach (var item7 in story.Filtro!.Where(str => str is CamadaOito && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                        {
                                            foreach (var item8 in story.Filtro!.Where(str => str is CamadaNove && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                            {
                                                var lista1 = (List<SubStory>)revertLista(story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                var lista2 = (List<Grupo>)revertLista(story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                var lista3 = (List<SubGrupo>)revertLista(story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                var lista4 = (List<SubSubGrupo>)revertLista(story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                var lista5 = (List<CamadaSeis>)revertLista(story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                var lista6 = (List<CamadaSete>)revertLista(story.Filtro!.Where(str => str is CamadaSete && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                var lista7 = (List<CamadaOito>)revertLista(story.Filtro!.Where(str => str is CamadaOito && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                var lista8 = (List<CamadaNove>)revertLista(story.Filtro!.Where(str => str is CamadaNove && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                                var var1 = lista1.IndexOf((SubStory)item) + 1;
                                                var var2 = lista2.IndexOf((Grupo)item2) + 1;
                                                var var3 = lista3.IndexOf((SubGrupo)item3) + 1;
                                                var var4 = lista4.IndexOf((SubSubGrupo)item4) + 1;
                                                var var5 = lista5.IndexOf((CamadaSeis)item5) + 1;
                                                var var6 = lista6.IndexOf((CamadaSete)item6) + 1;
                                                var var7 = lista7.IndexOf((CamadaOito)item7) + 1;
                                                var var8 = lista8.IndexOf((CamadaNove)item8) + 1;
                                                long num2 = long.Parse($"{Indice}{var1}{var2}{var3}{var4}{var5}{var6}{var7}{var8}");
                                                if (comparacao == 1)
                                                {
                                                    if (num2 > num)
                                                    {
                                                        condicao = true;
                                                        result = new int[5];
                                                        result[0] = Indice;
                                                        result[1] = var1;
                                                        result[2] = var2;
                                                        result[3] = var3;
                                                        result[4] = var4;
                                                        result[5] = var5;
                                                        result[6] = var6;
                                                        result[7] = var7;
                                                        result[8] = var8;
                                                        break;
                                                    }
                                                }
                                                else if (comparacao == 2)
                                                {
                                                        lista1.Reverse();
                                                        lista2.Reverse();
                                                        lista3.Reverse();
                                                        lista4.Reverse();
                                                        lista5.Reverse();
                                                        lista6.Reverse();
                                                        lista7.Reverse();
                                                        lista8.Reverse();
                                                    if (num2 < num)
                                                    {
                                                        condicao = true;
                                                        result = new int[5];
                                                        result[0] = Indice;
                                                        result[1] = var1;
                                                        result[2] = var2;
                                                        result[3] = var3;
                                                        result[4] = var4;
                                                        result[5] = var5;
                                                        result[6] = var6;
                                                        result[7] = var7;
                                                        result[8] = var8;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (item8.Id == filtro)
                                                    {
                                                        condicao = true;
                                                        result = new int[5];
                                                        result[0] = Indice;
                                                        result[1] = var1;
                                                        result[2] = var2;
                                                        result[3] = var3;
                                                        result[4] = var4;
                                                        result[5] = var5;
                                                        result[6] = var6;
                                                        result[7] = var7;
                                                        result[8] = var8;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (condicao) break;

                                    }
                                    if (condicao) break;
                                }
                                if (condicao) break;

                            }
                            if (condicao) break;

                        }
                        if (condicao) break;

                    }
                    if (condicao) break;
                }
            }
            else
            if (IndiceCamadaOito != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}{IndiceSubGrupo}{IndiceSubSubGrupo}{IndiceCamadaSeis}{IndiceCamadaSete}{IndiceCamadaOito}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    foreach (var item2 in story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                    {
                        foreach (var item3 in story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                        {
                            foreach (var item4 in story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                            {
                                foreach (var item5 in story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                {
                                    foreach (var item6 in story.Filtro!.Where(str => str is CamadaSete && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                    {
                                        foreach (var item7 in story.Filtro!.Where(str => str is CamadaOito && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                        {
                                            var lista1 = (List<SubStory>)revertLista(story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                            var lista2 = (List<Grupo>)revertLista(story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                            var lista3 = (List<SubGrupo>)revertLista(story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                            var lista4 = (List<SubSubGrupo>)revertLista(story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                            var lista5 = (List<CamadaSeis>)revertLista(story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                            var lista6 = (List<CamadaSete>)revertLista(story.Filtro!.Where(str => str is CamadaSete && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                            var lista7 = (List<CamadaOito>)revertLista(story.Filtro!.Where(str => str is CamadaOito && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                            var var1 = lista1.IndexOf((SubStory)item) + 1;
                                            var var2 = lista2.IndexOf((Grupo)item2) + 1;
                                            var var3 = lista3.IndexOf((SubGrupo)item3) + 1;
                                            var var4 = lista4.IndexOf((SubSubGrupo)item4) + 1;
                                            var var5 = lista5.IndexOf((CamadaSeis)item5) + 1;
                                            var var6 = lista6.IndexOf((CamadaSete)item6) + 1;
                                            var var7 = lista7.IndexOf((CamadaOito)item7) + 1;
                                            long num2 = long.Parse($"{Indice}{var1}{var2}{var3}{var4}{var5}{var6}{var7}");
                                            if (comparacao == 1)
                                            {
                                                if (num2 > num)
                                                {
                                                    condicao = true;
                                                    result = new int[5];
                                                    result[0] = Indice;
                                                    result[1] = var1;
                                                    result[2] = var2;
                                                    result[3] = var3;
                                                    result[4] = var4;
                                                    result[5] = var5;
                                                    result[6] = var6;
                                                    result[7] = var7;
                                                    break;
                                                }
                                            }
                                            else if (comparacao == 2)
                                            {
                                                    lista1.Reverse();
                                                    lista2.Reverse();
                                                    lista3.Reverse();
                                                    lista4.Reverse();
                                                    lista5.Reverse();
                                                    lista6.Reverse();
                                                    lista7.Reverse();
                                                if (num2 < num)
                                                {
                                                    condicao = true;
                                                    result = new int[5];
                                                    result[0] = Indice;
                                                    result[1] = var1;
                                                    result[2] = var2;
                                                    result[3] = var3;
                                                    result[4] = var4;
                                                    result[5] = var5;
                                                    result[6] = var6;
                                                    result[7] = var7;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (item7.Id == filtro)
                                                {
                                                    condicao = true;
                                                    result = new int[5];
                                                    result[0] = Indice;
                                                    result[1] = var1;
                                                    result[2] = var2;
                                                    result[3] = var3;
                                                    result[4] = var4;
                                                    result[5] = var5;
                                                    result[6] = var6;
                                                    result[7] = var7;
                                                    break;
                                                }
                                            }
                                        }
                                        if (condicao) break;

                                    }
                                    if (condicao) break;
                                }
                                if (condicao) break;

                            }
                            if (condicao) break;

                        }
                        if (condicao) break;

                    }
                    if (condicao) break;
                }
            }
            else
            if (IndiceCamadaSete != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}{IndiceSubGrupo}{IndiceSubSubGrupo}{IndiceCamadaSeis}{IndiceCamadaSete}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    foreach (var item2 in story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                    {
                        foreach (var item3 in story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                        {
                            foreach (var item4 in story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                            {
                                foreach (var item5 in story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                {
                                    foreach (var item6 in story.Filtro!.Where(str => str is CamadaSete && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                    {
                                        var lista1 = (List<SubStory>)revertLista(story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                        var lista2 = (List<Grupo>)revertLista(story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                        var lista3 = (List<SubGrupo>)revertLista(story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                        var lista4 = (List<SubSubGrupo>)revertLista(story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                        var lista5 = (List<CamadaSeis>)revertLista(story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                        var lista6 = (List<CamadaSete>)revertLista(story.Filtro!.Where(str => str is CamadaSete && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                        var var1 = lista1.IndexOf((SubStory)item) + 1;
                                        var var2 = lista2.IndexOf((Grupo)item2) + 1;
                                        var var3 = lista3.IndexOf((SubGrupo)item3) + 1;
                                        var var4 = lista4.IndexOf((SubSubGrupo)item4) + 1;
                                        var var5 = lista5.IndexOf((CamadaSeis)item5) + 1;
                                        var var6 = lista6.IndexOf((CamadaSete)item6) + 1;
                                        long num2 = long.Parse($"{Indice}{var1}{var2}{var3}{var4}{var5}{var6}");
                                        if (comparacao == 1)
                                        {
                                            if (num2 > num)
                                            {
                                                condicao = true;
                                                result = new int[5];
                                                result[0] = Indice;
                                                result[1] = var1;
                                                result[2] = var2;
                                                result[3] = var3;
                                                result[4] = var4;
                                                result[5] = var5;
                                                result[6] = var6;
                                                break;
                                            }
                                        }
                                        else if (comparacao == 2)
                                        {
                                                lista1.Reverse();
                                                lista2.Reverse();
                                                lista3.Reverse();
                                                lista4.Reverse();
                                                lista5.Reverse();
                                                lista6.Reverse();
                                            if (num2 < num)
                                            {
                                                condicao = true;
                                                result = new int[5];
                                                result[0] = Indice;
                                                result[1] = var1;
                                                result[2] = var2;
                                                result[3] = var3;
                                                result[4] = var4;
                                                result[5] = var5;
                                                result[6] = var6;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (item6.Id == filtro)
                                            {
                                                condicao = true;
                                                result = new int[5];
                                                result[0] = Indice;
                                                result[1] = var1;
                                                result[2] = var2;
                                                result[3] = var3;
                                                result[4] = var4;
                                                result[5] = var5;
                                                result[6] = var6;
                                                break;
                                            }
                                        }

                                    }
                                    if (condicao) break;
                                }
                                if (condicao) break;

                            }
                            if (condicao) break;

                        }
                        if (condicao) break;

                    }
                    if (condicao) break;
                }
            }
            else
            if (IndiceCamadaSeis != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}{IndiceSubGrupo}{IndiceSubSubGrupo}{IndiceCamadaSeis}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    foreach (var item2 in story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                    {
                        foreach (var item3 in story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                        {
                            foreach (var item4 in story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                            {
                                foreach (var item5 in story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                                {
                                    var lista1 = (List<SubStory>)revertLista(story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                    var lista2 = (List<Grupo>)revertLista(story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                    var lista3 = (List<SubGrupo>)revertLista(story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                    var lista4 = (List<SubSubGrupo>)revertLista(story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                    var lista5 = (List<CamadaSeis>)revertLista(story.Filtro!.Where(str => str is CamadaSeis && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                    var var1 = lista1.IndexOf((SubStory)item) + 1;
                                    var var2 = lista2.IndexOf((Grupo)item2) + 1;
                                    var var3 = lista3.IndexOf((SubGrupo)item3) + 1;
                                    var var4 = lista4.IndexOf((SubSubGrupo)item4) + 1;
                                    var var5 = lista5.IndexOf((CamadaSeis)item5) + 1;
                                    long num2 = long.Parse($"{Indice}{var1}{var2}{var3}{var4}{var5}");
                                    if (comparacao == 1)
                                    {
                                        if (num2 > num)
                                        {
                                            condicao = true;
                                            result = new int[5];
                                            result[0] = Indice;
                                            result[1] = var1;
                                            result[2] = var2;
                                            result[3] = var3;
                                            result[4] = var4;
                                            result[5] = var5;
                                            break;
                                        }
                                    }
                                    else if (comparacao == 2)
                                    {
                                            lista1.Reverse();
                                            lista2.Reverse();
                                            lista3.Reverse();
                                            lista4.Reverse();
                                            lista5.Reverse();
                                        if (num2 < num)
                                        {
                                            condicao = true;
                                            result = new int[5];
                                            result[0] = Indice;
                                            result[1] = var1;
                                            result[2] = var2;
                                            result[3] = var3;
                                            result[4] = var4;
                                            result[5] = var5;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (item5.Id == filtro)
                                        {
                                            condicao = true;
                                            result = new int[5];
                                            result[0] = Indice;
                                            result[1] = var1;
                                            result[2] = var2;
                                            result[3] = var3;
                                            result[4] = var4;
                                            result[5] = var5;
                                            break;
                                        }
                                    }
                                }
                                if (condicao) break;

                            }
                            if (condicao) break;

                        }
                        if (condicao) break;

                    }
                    if (condicao) break;
                }
            }
            else
            if (IndiceSubSubGrupo != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}{IndiceSubGrupo}{IndiceSubSubGrupo}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    foreach (var item2 in story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                    {
                        foreach (var item3 in story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                        {
                            foreach (var item4 in story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                            {
                                var lista1 = (List<SubStory>)revertLista(story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                var lista2 = (List<Grupo>)revertLista(story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                var lista3 = (List<SubGrupo>)revertLista(story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                var lista4 = (List<SubSubGrupo>)revertLista(story.Filtro!.Where(str => str is SubSubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                                var var1 = lista1.IndexOf((SubStory)item) + 1;
                                var var2 = lista2.IndexOf((Grupo)item2) + 1;
                                var var3 = lista3.IndexOf((SubGrupo)item3) + 1;
                                var var4 = lista4.IndexOf((SubSubGrupo)item4) + 1;
                                long num2 = long.Parse($"{Indice}{var1}{var2}{var3}{var4}");
                                if (comparacao == 1)
                                {
                                    if (num2 > num)
                                    {
                                        condicao = true;
                                        result = new int[5];
                                        result[0] = Indice;
                                        result[1] = var1;
                                        result[2] = var2;
                                        result[3] = var3;
                                        result[4] = var4;
                                        break;
                                    }
                                }
                                else if (comparacao == 2)
                                {
                                        lista1.Reverse();
                                        lista2.Reverse();
                                        lista3.Reverse();
                                        lista4.Reverse();
                                    if (num2 < num)
                                    {
                                        condicao = true;
                                        result = new int[5];
                                        result[0] = Indice;
                                        result[1] = var1;
                                        result[2] = var2;
                                        result[3] = var3;
                                        result[4] = var4;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (item4.Id == filtro)
                                    {
                                        condicao = true;
                                        result = new int[5];
                                        result[0] = Indice;
                                        result[1] = var1;
                                        result[2] = var2;
                                        result[3] = var3;
                                        result[4] = var4;
                                        break;
                                    }
                                }

                            }
                            if (condicao) break;

                        }
                        if (condicao) break;

                    }
                    if (condicao) break;
                }
            }
           
            else if (IndiceSubGrupo != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}{IndiceSubGrupo}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    foreach (var item2 in story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                    {
                        foreach (var item3 in story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                        {
                            var lista1 = (List<SubStory>)revertLista(story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                            var lista2 = (List<Grupo>)revertLista(story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                            var lista3 = (List<SubGrupo>)revertLista(story.Filtro!.Where(str => str is SubGrupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                            var var1 = lista1.IndexOf((SubStory)item) + 1;
                            var var2 = lista2.IndexOf((Grupo)item2) + 1;
                            var var3 = lista3.IndexOf((SubGrupo)item3) + 1;
                            long num2 = long.Parse($"{Indice}{var1}{var2}{var3}");
                            if (comparacao == 1)
                            {
                                if (num2 > num)
                                {
                                    condicao = true;
                                    result = new int[4];
                                    result[0] = Indice;
                                    result[1] = var1;
                                    result[2] = var2;
                                    result[3] = var3;
                                    break;
                                }
                            }
                            else if (comparacao == 2)
                            {
                                    lista1.Reverse();
                                    lista2.Reverse();
                                    lista3.Reverse();
                                if (num2 < num)
                                {
                                    condicao = true;
                                    result = new int[5];
                                    result[0] = Indice;
                                    result[1] = var1;
                                    result[2] = var2;
                                    result[3] = var3;
                                    break;
                                }
                            }
                            else
                            {
                                if (item3.Id == filtro)
                                {
                                    condicao = true;
                                    result = new int[4];
                                    result[0] = Indice;
                                    result[1] = var1;
                                    result[2] = var2;
                                    result[3] = var3;
                                    break;
                                }
                            }
                        }
                        if (condicao) break;

                    }
                    if (condicao) break;
                }
            }
          
            else if (IndiceGrupo != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    foreach (var item2 in story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                    {
                        var lista1 = (List<SubStory>)revertLista(story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                        var lista2 = (List<Grupo>)revertLista(story.Filtro!.Where(str => str is Grupo && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                        var var1 = lista1.IndexOf((SubStory)item) + 1;
                        var var2 = lista2.IndexOf((Grupo)item2) + 1;
                        long num2 = long.Parse($"{Indice}{var1}{var2}");
                        if (comparacao == 1)
                        {
                            if (num2 > num)
                            {
                                condicao = true;
                                result = new int[3];
                                result[0] = Indice;
                                result[1] = var1;
                                result[2] = var2;
                                break;
                            }
                        }
                        else if (comparacao == 2)
                        {
                                lista1.Reverse();
                                lista2.Reverse();
                            if (num2 < num)
                            {
                                condicao = true;
                                result = new int[5];
                                result[0] = Indice;
                                result[1] = var1;
                                result[2] = var2;
                                break;
                            }
                        }
                        else
                        {
                            if (item2.Id == filtro)
                            {
                                condicao = true;
                                result = new int[3];
                                result[0] = Indice;
                                result[1] = var1;
                                result[2] = var2;
                                break;
                            }
                        }
                    }
                    if (condicao) break;
                }
            }
          
            else
            {
                num = long.Parse($"{Indice}{IndiceSubStory}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    var lista1 = (List<SubStory>)revertLista(story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList(), comparacao);
                    var var1 = lista1.IndexOf((SubStory)item) + 1;
                    long num2 = long.Parse($"{Indice}{var1}");
                    if (comparacao == 1)
                    {
                        if (num2 > num)
                        {
                            condicao = true;
                            result = new int[2];
                            result[0] = Indice;
                            result[1] = var1;
                            break;
                        }
                    }
                    else if (comparacao == 2)
                    {
                        lista1.Reverse();
                        if (num2 < num)
                        {
                            condicao = true;
                            result = new int[5];
                            result[0] = Indice;
                            result[1] = var1;
                            break;
                        }
                    }
                    else
                    {
                        if (item.Id == filtro)
                        {
                            condicao = true;
                            result = new int[2];
                            result[0] = Indice;
                            result[1] = var1;
                            break;
                        }
                    }
                }
            }
          
            return result!;
        }

        private object revertLista(object lista , int comparacao)
        {
            if(comparacao == 2)
            {
                if(lista is IList)
                {
                    IList<int> list = (IList<int>)lista;
                    list.Reverse();
                }
            }
                return lista;
        }
    
    
    }

}