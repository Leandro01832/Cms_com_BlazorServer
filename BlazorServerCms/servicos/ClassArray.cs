
using System.Linq;
using System;
using System.Collections.Generic;
using business.Group;
using System.Collections;
using business;
using PSC.Blazor.Components.Tours.Interfaces;

namespace BlazorServerCms.servicos
{
    public class ClassArray
    {

        public int[] RetornarArray(List<Filtro> filtros, Story story, int comparacao, long filtro,
            int Indice, int IndiceSubStory, int? IndiceGrupo = null, int? IndiceSubGrupo = null,
            int? IndiceSubSubGrupo = null, int? IndiceCamadaSeis = null, int? IndiceCamadaSete = null,
          int? IndiceCamadaOito = null, int? IndiceCamadaNove = null, int? IndiceCamadaDez = null)
        {
            int[]? result = null;
            bool condicao = false;
            bool possuiVersos = false;
            long num;

            if (comparacao == 2 &&
                story.Filtro.FirstOrDefault(f => f.Id == filtro && f.Pagina != null && f.Pagina!.Count > 0) != null)
                possuiVersos = true;

            if (!possuiVersos && comparacao == 2) return null;
            else
            if (possuiVersos && comparacao == 2) return new int[1];
            else if (IndiceCamadaDez != null)
            {
                num = long.Parse($"{Indice}{IndiceSubStory}{IndiceGrupo}{IndiceSubGrupo}{IndiceSubSubGrupo}{IndiceCamadaSeis}{IndiceCamadaSete}{IndiceCamadaOito}{IndiceCamadaNove}{IndiceCamadaDez}");
                foreach (var item in story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList())
                {
                    var fil1 = (SubStory) filtros.First(f => f.Id == item.Id);
                    if (fil1.Grupo != null)
                        foreach (var item2 in fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                    {
                        var fil2 = (Grupo) filtros.First(f => f.Id == item2.Id);
                            if (fil2.SubGrupo != null)
                                foreach (var item3 in fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                        {
                            var fil3 = (SubGrupo) filtros.First(f => f.Id == item3.Id);
                                    if (fil3.SubSubGrupo != null)
                                        foreach (var item4 in fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                            {
                                var fil4 = (SubSubGrupo) filtros.First(f => f.Id == item4.Id);
                                            if (fil4.CamadaSeis != null)
                                                foreach (var item5 in fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                {
                                    var fil5 = (CamadaSeis) filtros.First(f => f.Id == item5.Id);
                                                    if (fil5.CamadaSete != null)
                                                        foreach (var item6 in fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                    {
                                         var fil6 = (CamadaSete) filtros.First(f => f.Id == item6.Id);
                                                            if (fil6.CamadaOito != null)
                                                                foreach (var item7 in fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                        {
                                             var fil7 = (CamadaOito) filtros.First(f => f.Id == item7.Id);
                                                                        if (fil7.CamadaNove != null)
                                            foreach (var item8 in fil7.CamadaNove.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                            {
                                                var fil8 = (CamadaNove) filtros.First(f => f.Id == item7.Id);
                                                                            if (fil8.CamadaDez != null)
                                                                                foreach (var item9 in fil8.CamadaDez.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                                {
                                                    var lista1 =  story.Filtro!.Where(str => str is SubStory    && str.Pagina != null && str.Pagina!.Count > 0).ToList();
                                                    var lista2 =  fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                    var lista3 =  fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                    var lista4 =  fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                    var lista5 =  fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                    var lista6 =  fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                    var lista7 =  fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                    var lista8 =  fil7.CamadaNove.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                    var lista9 =  fil8.CamadaDez.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                   
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
                                                            result = new int[10];
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
                                                            result = new int[10];
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
                    var fil1 = (SubStory)filtros.First(f => f.Id == item.Id);
                    if (fil1.Grupo != null)
                        foreach (var item2 in fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                    {
                        var fil2 = (Grupo)filtros.First(f => f.Id == item2.Id);
                            if (fil2.SubGrupo != null)
                                foreach (var item3 in fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                        {
                            var fil3 = (SubGrupo)filtros.First(f => f.Id == item3.Id);
                                    if (fil3.SubSubGrupo != null)
                                        foreach (var item4 in fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                            {
                                var fil4 = (SubSubGrupo)filtros.First(f => f.Id == item4.Id);
                                            if (fil4.CamadaSeis != null)
                                                foreach (var item5 in fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                {
                                    var fil5 = (CamadaSeis)filtros.First(f => f.Id == item5.Id);
                                                    if (fil5.CamadaSete != null)
                                                        foreach (var item6 in fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                    {
                                        var fil6 = (CamadaSete)filtros.First(f => f.Id == item6.Id);
                                                            if (fil6.CamadaOito != null)
                                                                foreach (var item7 in fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                        {
                                            var fil7 = (CamadaOito)filtros.First(f => f.Id == item7.Id);
                                                                    if (fil7.CamadaNove != null)
                                                                        foreach (var item8 in fil7.CamadaNove.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                            {
                                                var lista1 = story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList()   ;
                                                var lista2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                var lista3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                var lista4 = fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                var lista5 = fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                var lista6 = fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                var lista7 = fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                                var lista8 = fil7.CamadaNove.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                              
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
                                                        result = new int[9];
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
                                                        result = new int[9];
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
                    var fil1 = (SubStory)filtros.First(f => f.Id == item.Id);
                    if (fil1.Grupo != null)
                        foreach (var item2 in fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                    {
                        var fil2 = (Grupo)filtros.First(f => f.Id == item2.Id);
                            if (fil2.SubGrupo != null)
                                foreach (var item3 in fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                        {
                            var fil3 = (SubGrupo)filtros.First(f => f.Id == item3.Id);
                                    if (fil3.SubSubGrupo != null)
                                        foreach (var item4 in fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                            {
                                var fil4 = (SubSubGrupo)filtros.First(f => f.Id == item4.Id);
                                            if (fil4.CamadaSeis != null)
                                                foreach (var item5 in fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                {
                                    var fil5 = (CamadaSeis)filtros.First(f => f.Id == item5.Id);
                                                    if (fil5.CamadaSete != null)
                                                        foreach (var item6 in fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                    {
                                        var fil6 = (CamadaSete)filtros.First(f => f.Id == item6.Id);
                                                            if(fil6.CamadaOito != null)
                                        foreach (var item7 in fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                        {
                                            var lista1 = story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList();
                                            var lista2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                            var lista3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                            var lista4 = fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                            var lista5 = fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                            var lista6 = fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                            var lista7 = fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                          
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
                                                    result = new int[8];
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
                                                    result = new int[8];
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
                    var fil1 = (SubStory)filtros.First(f => f.Id == item.Id);
                    if (fil1.Grupo != null)
                        foreach (var item2 in fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                    {
                        var fil2 = (Grupo)filtros.First(f => f.Id == item2.Id);
                            if (fil2.SubGrupo != null)
                                foreach (var item3 in fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                        {
                            var fil3 = (SubGrupo)filtros.First(f => f.Id == item3.Id);
                                    if (fil3.SubSubGrupo != null)
                                        foreach (var item4 in fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                            {
                                var fil4 = (SubSubGrupo)filtros.First(f => f.Id == item4.Id);
                                            if (fil4.CamadaSeis != null)
                                                foreach (var item5 in fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                {
                                    var fil5 = (CamadaSeis)filtros.First(f => f.Id == item5.Id);
                                                    if(fil5.CamadaSete != null)
                                    foreach (var item6 in fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                    {
                                        var lista1 = story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList()   ;
                                        var lista2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                        var lista3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                        var lista4 = fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                        var lista5 = fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                        var lista6 = fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                        
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
                                                result = new int[7];
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
                                                result = new int[7];
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
                    var fil1 = (SubStory)filtros.First(f => f.Id == item.Id);
                                if(fil1.Grupo != null)
                    foreach (var item2 in fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                    {
                        var fil2 = (Grupo)filtros.First(f => f.Id == item2.Id);
                                if(fil2.SubGrupo != null)
                        foreach (var item3 in fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                        {
                            var fil3 = (SubGrupo)filtros.First(f => f.Id == item3.Id);
                                if(fil3.SubSubGrupo != null)
                            foreach (var item4 in fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                            {
                                var fil4 = (SubSubGrupo)filtros.First(f => f.Id == item4.Id);
                                if(fil4.CamadaSeis != null)
                                foreach (var item5 in fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                                {
                                    var lista1 = story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList()    ;
                                    var lista2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                    var lista3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                    var lista4 = fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                    var lista5 = fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                    
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
                                            result = new int[6];
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
                                            result = new int[6];
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
                    var fil1 = (SubStory)filtros.First(f => f.Id == item.Id);
                    if (fil1.Grupo != null)
                        foreach (var item2 in fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                    {
                        var fil2 = (Grupo)filtros.First(f => f.Id == item2.Id);
                            if (fil2.SubGrupo != null)
                                foreach (var item3 in fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                        {
                            var fil3 = (SubGrupo)filtros.First(f => f.Id == item3.Id);
                                    if (fil3.SubSubGrupo != null)
                                        foreach (var item4 in fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                            {
                                var lista1 = story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList();
                                var lista2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                var lista3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                                var lista4 = fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList().OrderBy(str => str.Id).ToList();
                               
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
                    var fil1 = (SubStory)filtros.First(f => f.Id == item.Id);
                    if (fil1.Grupo != null)
                        foreach (var item2 in fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                    {
                        var fil2 = (Grupo)filtros.First(f => f.Id == item2.Id);
                            if (fil2.SubGrupo != null)
                                foreach (var item3 in fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList())
                        {
                            var lista1 = story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList() ;
                            var lista2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                            var lista3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList();
                           
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
                    var fil1 = (SubStory)filtros.First(f => f.Id == item.Id);
                    if (fil1.Grupo != null)
                        foreach (var item2 in fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList().ToList())
                    {
                        var lista1 = story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList();
                        var lista2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).ToList().ToList();
                        
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
                    var lista1 = story.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).ToList();
                   
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

       
    
    
    }

}