
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
            if (comparacao == 4)            
            {
                if (indices[9] != null)
                {
                    foreach (var camadadez in story.Filtro.OfType<CamadaDez>().Where(HasPages))
                    {
                        if (camadadez.Id == filtro)
                        {
                            var camada9 = story.Filtro.OfType<CamadaNove>().First(str => str.Id == camadadez.CamadaNoveId);
                            var camada8 = story.Filtro.OfType<CamadaOito>().First(str => str.Id == camada9.CamadaOitoId);
                            var camada7 = story.Filtro.OfType<CamadaSete>().First(str => str.Id == camada8.CamadaSeteId);
                            var camada6 = story.Filtro.OfType<CamadaSeis>().First(str => str.Id == camada7.CamadaSeisId);
                            var camada5 = story.Filtro.OfType<SubSubGrupo>().First(str => str.Id == camada6.SubSubGrupoId);
                            var camada4 = story.Filtro.OfType<SubGrupo>().First(str => str.Id == camada5.SubGrupoId);
                            var camada3 = story.Filtro.OfType<Grupo>().First(str => str.Id == camada4.GrupoId);
                            var camada2 = story.Filtro.OfType<SubStory>().First(str => str.Id == camada3.SubStoryId);

                            var dezIndex = GetIndex(camada9.CamadaDez!, camadadez);
                            var noveIndex = GetIndex(camada8.CamadaNove!, camada9);
                            var oitoIndex = GetIndex(camada7.CamadaOito!, camada8);
                            var seteIndex = GetIndex(camada6.CamadaSete!, camada7);
                            var seisIndex = GetIndex(camada5.CamadaSeis!, camada6);
                            var subSubGrupoIndex = GetIndex(camada4.SubSubGrupo!, camada5);
                            var subGrupoIndex = GetIndex(camada3.SubGrupo!, camada4);
                            var grupoIndex = GetIndex(camada2.Grupo!, camada3);
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex,
                              subSubGrupoIndex, seisIndex, seteIndex, oitoIndex, noveIndex, dezIndex);
                            return;

                        }                    
                    }
                }
                else if (indices[8] != null)
                {
                    foreach (var camada9 in story.Filtro.OfType<CamadaNove>())
                    {
                        if (camada9.Id == filtro)
                        {
                            var camada8 = story.Filtro.OfType<CamadaOito>().First(str => str.Id == camada9.CamadaOitoId);
                            var camada7 = story.Filtro.OfType<CamadaSete>().First(str => str.Id == camada8.CamadaSeteId);
                            var camada6 = story.Filtro.OfType<CamadaSeis>().First(str => str.Id == camada7.CamadaSeisId);
                            var camada5 = story.Filtro.OfType<SubSubGrupo>().First(str => str.Id == camada6.SubSubGrupoId);
                            var camada4 = story.Filtro.OfType<SubGrupo>().First(str => str.Id == camada5.SubGrupoId);
                            var camada3 = story.Filtro.OfType<Grupo>().First(str => str.Id == camada4.GrupoId);
                            var camada2 = story.Filtro.OfType<SubStory>().First(str => str.Id == camada3.SubStoryId);

                            var noveIndex = GetIndex(camada8.CamadaNove!, camada9);
                            var oitoIndex = GetIndex(camada7.CamadaOito!, camada8);
                            var seteIndex = GetIndex(camada6.CamadaSete!, camada7);
                            var seisIndex = GetIndex(camada5.CamadaSeis!, camada6);
                            var subSubGrupoIndex = GetIndex(camada4.SubSubGrupo!, camada5);
                            var subGrupoIndex = GetIndex(camada3.SubGrupo!, camada4);
                            var grupoIndex = GetIndex(camada2.Grupo!, camada3);
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex,
                              subSubGrupoIndex, seisIndex, seteIndex, oitoIndex, noveIndex);
                            return;
                        }
                    }
                }
                else if (indices[7] != null)
                {
                    foreach (var camada8 in story.Filtro.OfType<CamadaOito>())
                    {
                        if (camada8.Id == filtro)
                        {
                            var camada7 = story.Filtro.OfType<CamadaSete>().First(str => str.Id == camada8.CamadaSeteId);
                            var camada6 = story.Filtro.OfType<CamadaSeis>().First(str => str.Id == camada7.CamadaSeisId);
                            var camada5 = story.Filtro.OfType<SubSubGrupo>().First(str => str.Id == camada6.SubSubGrupoId);
                            var camada4 = story.Filtro.OfType<SubGrupo>().First(str => str.Id == camada5.SubGrupoId);
                            var camada3 = story.Filtro.OfType<Grupo>().First(str => str.Id == camada4.GrupoId);
                            var camada2 = story.Filtro.OfType<SubStory>().First(str => str.Id == camada3.SubStoryId);

                            var oitoIndex = GetIndex(camada7.CamadaOito!, camada8);
                            var seteIndex = GetIndex(camada6.CamadaSete!, camada7);
                            var seisIndex = GetIndex(camada5.CamadaSeis!, camada6);
                            var subSubGrupoIndex = GetIndex(camada4.SubSubGrupo!, camada5);
                            var subGrupoIndex = GetIndex(camada3.SubGrupo!, camada4);
                            var grupoIndex = GetIndex(camada2.Grupo!, camada3);
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex,
                               subSubGrupoIndex, seisIndex, seteIndex, oitoIndex);
                            return;
                        }
                    }
                }
                else if (indices[6] != null)
                {
                    foreach (var camada7 in story.Filtro.OfType<CamadaSete>())
                    {
                        if (camada7.Id == filtro)
                        {
                            var camada6 = story.Filtro.OfType<CamadaSeis>().First(str => str.Id == camada7.CamadaSeisId);
                            var camada5 = story.Filtro.OfType<SubSubGrupo>().First(str => str.Id == camada6.SubSubGrupoId);
                            var camada4 = story.Filtro.OfType<SubGrupo>().First(str => str.Id == camada5.SubGrupoId);
                            var camada3 = story.Filtro.OfType<Grupo>().First(str => str.Id == camada4.GrupoId);
                            var camada2 = story.Filtro.OfType<SubStory>().First(str => str.Id == camada3.SubStoryId);

                            var seteIndex = GetIndex(camada6.CamadaSete!, camada7);
                            var seisIndex = GetIndex(camada5.CamadaSeis!, camada6);
                            var subSubGrupoIndex = GetIndex(camada4.SubSubGrupo!, camada5);
                            var subGrupoIndex = GetIndex(camada3.SubGrupo!, camada4);
                            var grupoIndex = GetIndex(camada2.Grupo!, camada3);
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex,
                                subSubGrupoIndex, seisIndex, seteIndex);
                            return;
                        }
                    }
                }
                else if (indices[5] != null)
                {
                    foreach (var camada6 in story.Filtro.OfType<CamadaSeis>())
                    {
                        if (camada6.Id == filtro)
                        {
                            var camada5 = story.Filtro.OfType<SubSubGrupo>().First(str => str.Id == camada6.SubSubGrupoId);
                            var camada4 = story.Filtro.OfType<SubGrupo>().First(str => str.Id == camada5.SubGrupoId);
                            var camada3 = story.Filtro.OfType<Grupo>().First(str => str.Id == camada4.GrupoId);
                            var camada2 = story.Filtro.OfType<SubStory>().First(str => str.Id == camada3.SubStoryId);

                            var seisIndex = GetIndex(camada5.CamadaSeis!, camada6);
                            var subSubGrupoIndex = GetIndex(camada4.SubSubGrupo!, camada5);
                            var subGrupoIndex = GetIndex(camada3.SubGrupo!, camada4);
                            var grupoIndex = GetIndex(camada2.Grupo!, camada3);
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex, seisIndex);
                            return;
                        }
                    }
                }
                else if (indices[4] != null)
                {
                    foreach (var camada5 in story.Filtro.OfType<SubSubGrupo>())
                    {
                        if (camada5.Id == filtro)
                        {
                            var camada4 = story.Filtro.OfType<SubGrupo>().First(str => str.Id == camada5.SubGrupoId);
                            var camada3 = story.Filtro.OfType<Grupo>().First(str => str.Id == camada4.GrupoId);
                            var camada2 = story.Filtro.OfType<SubStory>().First(str => str.Id == camada3.SubStoryId);

                            var subSubGrupoIndex = GetIndex(camada4.SubSubGrupo!, camada5);
                            var subGrupoIndex = GetIndex(camada3.SubGrupo!, camada4);
                            var grupoIndex = GetIndex(camada2.Grupo!, camada3);
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex, subSubGrupoIndex);
                            return;
                        }
                    }
                }
                else if (indices[3] != null)
                {
                    foreach (var camada4 in story.Filtro.OfType<SubGrupo>())
                    {
                        if (camada4.Id == filtro)
                        {
                            var camada3 = story.Filtro.OfType<Grupo>().First(str => str.Id == camada4.GrupoId);
                            var camada2 = story.Filtro.OfType<SubStory>().First(str => str.Id == camada3.SubStoryId);

                            var subGrupoIndex = GetIndex(camada3.SubGrupo!, camada4);
                            var grupoIndex = GetIndex(camada2.Grupo!, camada3);
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex, grupoIndex, subGrupoIndex);
                            return;
                        }
                    }
                }
                else if(indices[2] != null)
                {
                    foreach (var camada3 in story.Filtro.OfType<Grupo>())
                    {
                        if (camada3.Id == filtro)
                        {
                            var camada2 = story.Filtro.OfType<SubStory>().First(str => str.Id == camada3.SubStoryId);

                            var grupoIndex = GetIndex(camada2.Grupo!, camada3);
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex, grupoIndex);
                            return;
                        }
                    }
                }
                else if (indices[1] != null)
                {
                    foreach (var camada2 in story.Filtro.OfType<SubStory>())
                    {
                        if (camada2.Id == filtro)
                        {
                            var subStoryIndex = GetIndex(story.Filtro, camada2);
                            PopulateResult(result, subStoryIndex);
                            return;
                        }
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