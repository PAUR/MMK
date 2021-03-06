﻿using System;
using System.Collections.Generic;
using System.Linq;
using MMK.Marking;
using MMK.Marking.Representation;

namespace MMK.HotMark.Model
{
    public abstract class HashTagModelChangeModel
    {
        protected HashTagModelChangeModel(IEnumerable<string> paths, IEnumerable<HashTag> initialConjointHashTagModel)
        {
            Paths = paths as string[] ?? paths.ToArray();
            InitialConjointHashTagModel = new HashTagModel(initialConjointHashTagModel);
        }

        protected string[] Paths { get; private set; }

        protected HashTagModel InitialConjointHashTagModel { get; private set; }

        protected HashTagModel FinalHashTagModel { get; private set; }

        public void NotifyChange(IEnumerable<HashTag> hashTags = null)
        {
            if(hashTags != null)
                SetFinalHashTagModel(hashTags);

            if(FinalHashTagModel == null)
                throw new Exception("FinalHashTagModel must be setup");

            OnNotifyChange();
        }

        protected abstract void OnNotifyChange();

        public void SetFinalHashTagModel(IEnumerable<HashTag> hashTags)
        {
            FinalHashTagModel = new HashTagModel(hashTags);
        }
    }
}
