
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using business.Group;
using business;
using System.Text.RegularExpressions;

namespace BlazorServerCms.servicos
{
    public class ClassArray
    {
        public int?[] RetornarArray(List<Filtro> filtros, Story story, int comparacao, long filtro,
            int indice, int? indiceSubStory, int? indiceGrupo = null, int? indiceSubGrupo = null,
            int? indiceSubSubGrupo = null, int? indiceCamadaSeis = null, int? indiceCamadaSete = null,
          int? indiceCamadaOito = null, int? indiceCamadaNove = null, int? indiceCamadaDez = null)
        {
            bool firstLoop = false;
            int?[] result = new int?[10];
            long currentNum = BuildNumber(indice, indiceSubStory, indiceGrupo, indiceSubGrupo, indiceSubSubGrupo,
                indiceCamadaSeis, indiceCamadaSete, indiceCamadaOito, indiceCamadaNove, indiceCamadaDez);

            if (comparacao == 2 && !HasValidFilters(story, filtro))
                return null;

                 TraverseHierarchy(filtros, story, comparacao, filtro, currentNum, firstLoop, result, indice,
                 indiceSubStory, indiceGrupo, indiceSubGrupo, indiceSubSubGrupo,
                indiceCamadaSeis, indiceCamadaSete, indiceCamadaOito, indiceCamadaNove, indiceCamadaDez);

            if(result[1] == null)
            {
                firstLoop = true;
                if (indiceCamadaDez != null)    indiceCamadaDez = null;   else
                if(indiceCamadaNove != null)   indiceCamadaNove =  null; else
                if(indiceCamadaOito != null)   indiceCamadaOito =  null; else
                if(indiceCamadaSete != null)   indiceCamadaSete =  null; else
                if(indiceCamadaSeis != null)   indiceCamadaSeis =  null; else
                if(indiceSubSubGrupo != null)  indiceSubSubGrupo = null; else
                if(indiceSubGrupo != null)     indiceSubGrupo =    null; else
                if(indiceGrupo != null)        indiceGrupo =       null; else
                if(indiceSubStory != null)     {indiceSubStory =    null; firstLoop = false;  }

                TraverseHierarchy(filtros, story, comparacao, filtro, currentNum, firstLoop, result, indice,
                indiceSubStory, indiceGrupo, indiceSubGrupo, indiceSubSubGrupo,
               indiceCamadaSeis, indiceCamadaSete, indiceCamadaOito, indiceCamadaNove, indiceCamadaDez);
            }

            result[0] = indice;
            return result;
        }

        private long BuildNumber(params int?[] indices)
        {
            return long.Parse(string.Join("", indices.Select(i => i?.ToString() ?? "")));
        }

        private bool HasValidFilters(Story story, long filtro)
        {
            return story.Filtro.Any(f => f.Id == filtro && f.Pagina != null && f.Pagina.Count > 0);
        }

        private void TraverseHierarchy(List<Filtro> filtros, Story story, int comparacao, long filtro, long currentNum, bool firstLoop, int?[] result, params int?[] indices)
        {
            foreach (var subStory in story.Filtro.OfType<SubStory>().Where(HasPages))
            {
                var subStoryIndex = GetIndex(story.Filtro, subStory);
                if (subStory.Grupo != null && indices[2] != null)
                foreach (var grupo in subStory.Grupo.Where(HasPages))
                {
                    var grupoIndex = GetIndex(subStory.Grupo, grupo);
                    if(grupo.SubGrupo != null && indices[3] != null)
                    foreach (var subGrupo in grupo.SubGrupo.Where(HasPages))
                    {
                        var subGrupoIndex = GetIndex(grupo.SubGrupo, subGrupo);
                        if (subGrupo.SubSubGrupo != null && indices[4] != null)
                        foreach (var subSubGrupo in subGrupo.SubSubGrupo.Where(HasPages))
                        {
                            var subSubGrupoIndex = GetIndex(subGrupo.SubSubGrupo, subSubGrupo);
                            if (subSubGrupo.CamadaSeis != null && indices[5] != null)
                            foreach (var camadaSeis in subSubGrupo.CamadaSeis.Where(HasPages))
                            {
                                var camadaSeisIndex = GetIndex(subSubGrupo.CamadaSeis, camadaSeis);
                                if (camadaSeis.CamadaSete != null && indices[6] != null)
                                foreach (var camadaSete in camadaSeis.CamadaSete.Where(HasPages))
                                {
                                    var camadaSeteIndex = GetIndex(camadaSeis.CamadaSete, camadaSete);
                                    if (camadaSete.CamadaOito != null && indices[7] != null)
                                    foreach (var camadaOito in camadaSete.CamadaOito.Where(HasPages))
                                    {
                                        var camadaOitoIndex = GetIndex(camadaSete.CamadaOito, camadaOito);
                                        if (camadaOito.CamadaNove != null && indices[8] != null)
                                        foreach (var camadaNove in camadaOito.CamadaNove.Where(HasPages))
                                        {
                                            var camadaNoveIndex = GetIndex(camadaOito.CamadaNove, camadaNove);
                                            if (camadaNove.CamadaDez != null && indices[9] != null)
                                            foreach (var camadaDez in camadaNove.CamadaDez.Where(HasPages))
                                            {
                                                var camadaDezIndex = GetIndex(camadaNove.CamadaDez, camadaDez);
                                                long newNum = BuildNumber(indices[0], subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                    camadaSeisIndex, camadaSeteIndex, camadaOitoIndex, camadaNoveIndex, camadaDezIndex);
                                                    if (IsMatch(comparacao, filtro, currentNum, newNum, camadaDez.Id) || firstLoop)
                                                    {
                                                        PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                            camadaSeisIndex, camadaSeteIndex, camadaOitoIndex, camadaNoveIndex, camadaDezIndex);
                                                        return;
                                                    }
                                                
                                            }
                                           else
                                           {
                                                long newNum = BuildNumber(indices[0], subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                    camadaSeisIndex, camadaSeteIndex, camadaOitoIndex, camadaNoveIndex);
                                                    if (IsMatch(comparacao, filtro, currentNum, newNum, camadaNove.Id) || firstLoop)
                                                    {
                                                        PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                            camadaSeisIndex, camadaSeteIndex, camadaOitoIndex, camadaNoveIndex);
                                                        return;
                                                    }
                                           }
                                        }
                                        else
                                           {
                                                long newNum = BuildNumber(indices[0], subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                    camadaSeisIndex, camadaSeteIndex, camadaOitoIndex);
                                                    if (IsMatch(comparacao, filtro, currentNum, newNum, camadaOito.Id) || firstLoop)
                                                    {
                                                        PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                            camadaSeisIndex, camadaSeteIndex, camadaOitoIndex);
                                                        return;
                                                    }
                                           }
                                    }
                                    else
                                           {
                                                long newNum = BuildNumber(indices[0], subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                    camadaSeisIndex, camadaSeteIndex);
                                                    if (IsMatch(comparacao, filtro, currentNum, newNum, camadaSete.Id) || firstLoop)
                                                    {
                                                        PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                            camadaSeisIndex, camadaSeteIndex);
                                                        return;
                                                    }
                                           }
                                }
                                else
                                           {
                                                long newNum = BuildNumber(indices[0], subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                    camadaSeisIndex);
                                                    if (IsMatch(comparacao, filtro, currentNum, newNum, camadaSeis.Id) || firstLoop)
                                                    {
                                                        PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex,
                                                            camadaSeisIndex);
                                                        return;
                                                    }
                                           }
                            }
                             else
                                 {
                                     long newNum = BuildNumber(indices[0], subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex);
                                     if (IsMatch(comparacao, filtro, currentNum, newNum, subSubGrupo.Id) || firstLoop)
                                     {
                                         PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex);
                                         return;
                                     }
                                 }
                             }
                            else
                            {
                                long newNum = BuildNumber(indices[0], subStoryIndex, grupoIndex, subGrupoIndex);
                                if (IsMatch(comparacao, filtro, currentNum, newNum, subGrupo.Id) || firstLoop)
                                {
                                    PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex);
                                    return;
                                }
                            }
                        }
                    else
                    {
                        long newNum = BuildNumber(indices[0], subStoryIndex, grupoIndex);
                        if (IsMatch(comparacao, filtro, currentNum, newNum, grupo.Id) || firstLoop)
                        {
                            PopulateResult(result, subStoryIndex, grupoIndex);
                            return;
                        }
                    }
                }
                else
                {
                    long newNum = BuildNumber(indices[0], subStoryIndex);
                    if (IsMatch(comparacao, filtro, currentNum, newNum, subStory.Id) || firstLoop)
                    {
                        PopulateResult(result, subStoryIndex);
                        return;
                    }
                }

            }
        }

        private bool HasPages(Filtro filtro)
        {
            return filtro.Pagina != null && filtro.Pagina.Count > 0;
        }

        private int GetIndex<T>(IEnumerable<T> collection, T item)
        {
            return collection.ToList().IndexOf(item) + 1;
        }

        private bool IsMatch(int comparacao, long filtro, long currentNum, long newNum, long itemId)
        {
            return (comparacao == 1 && newNum > currentNum) || (comparacao == 3 && itemId == filtro);
        }

        private void PopulateResult(int?[] result, params int?[] indices)
        {
            for (int i = 1; i < indices.Length; i++)
            {
                result[i] = indices[i - 1];
            }
            result[indices.Length] = indices[indices.Length - 1];
        }
    }
}