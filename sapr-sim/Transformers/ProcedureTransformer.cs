﻿using sapr_sim.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTransformator.Transformers
{
    public class ProcedureTransformer : Transformer
    {

        public Entities.Entity transform(sapr_sim.Figures.UIEntity entity)
        {
            Procedure p = entity as Procedure;
            return new Entities.impl.Procedure() { manHour = p.ManHour, name = p.EntityName };
        }
    }
}
