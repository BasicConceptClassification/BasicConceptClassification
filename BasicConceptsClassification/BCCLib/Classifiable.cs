﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCCLib
{
    public class Classifiable
    {
        public string id
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public string url
        {
            get;
            set;
        }
        public string tmpConceptStr
        {
            get;
            set;
        }
        public ConceptString conceptStr
        {
            get;
            set;
        }

        public Classifier owner
        {
            get;
            set;
        }
    }
}
