﻿using System;
using System.Collections.Generic;
using System.Linq;
using KolonyTools;
using UnityEngine;



namespace PlanetaryLogistics
{
    public class PlanetaryLogisticsManager : MonoBehaviour
    {
        // Static singleton instance
        private static PlanetaryLogisticsManager instance;

        // Static singleton property
        public static PlanetaryLogisticsManager Instance
        {
            get { return instance ?? (instance = new GameObject("PlanetaryLogisticsManager").AddComponent<PlanetaryLogisticsManager>()); }
        }

        //Backing variables
        private List<PlanetaryLogisticsEntry> _PlanetaryLogisticsInfo;

        public void ResetCache()
        {
            _PlanetaryLogisticsInfo = null;
        }

        public List<PlanetaryLogisticsEntry> PlanetaryLogisticsInfo
        {
            get
            {
                if (_PlanetaryLogisticsInfo == null)
                {
                    _PlanetaryLogisticsInfo = new List<PlanetaryLogisticsEntry>();
                    _PlanetaryLogisticsInfo.AddRange(PlanetaryLogisticsScenario.Instance.settings.GetStatusInfo());
                }
                return _PlanetaryLogisticsInfo;
            }
        }

        public bool DoesLogEntryExist(string vesselId, int body)
        {
            //Does a node exist?
            return PlanetaryLogisticsInfo.Any(n => n.vesselIdName == vesselId
                && n.BodyIndex == body);
        }

        public void RemoveLogEntry(string vesselId, int body)
        {
            if (!DoesLogEntryExist(vesselId,body))
                return;
            var logEntry = PlanetaryLogisticsInfo.First(n => n.vesselIdName == vesselId
                && n.BodyIndex == body);
            PlanetaryLogisticsInfo.Remove(logEntry);
            //For saving to our scenario data
            PlanetaryLogisticsScenario.Instance.settings.DeleteStatusNode(logEntry);
        }

        public PlanetaryLogisticsEntry FetchLogEntry(string vesselId, int body)
        {
            if (!DoesLogEntryExist(vesselId, body))
            {
                var k = new PlanetaryLogisticsEntry();
                k.vesselIdName = vesselId;
                k.BodyIndex = body;
                k.StoredQuantity = 0d;
                TrackLogEntry(k);
            }

            var logEntry = PlanetaryLogisticsInfo.FirstOrDefault(n => n.vesselIdName == vesselId
                && n.BodyIndex == body);
            return logEntry;
        }

        public void TrackLogEntry(PlanetaryLogisticsEntry logEntry)
        {
            PlanetaryLogisticsEntry newEntry =
                PlanetaryLogisticsInfo.FirstOrDefault(n => n.vesselIdName == logEntry.vesselIdName
                && n.BodyIndex == logEntry.BodyIndex);
            if (newEntry == null)
            {
                newEntry = new PlanetaryLogisticsEntry();
                newEntry.vesselIdName = logEntry.vesselIdName;
                newEntry.BodyIndex = logEntry.BodyIndex;
                PlanetaryLogisticsInfo.Add(newEntry);
            }
            newEntry.StoredQuantity = logEntry.StoredQuantity;
            PlanetaryLogisticsScenario.Instance.settings.SaveLogEntryNode(logEntry);
        }
    }
}

