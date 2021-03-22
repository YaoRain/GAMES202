using System;
using System.Collections;
using Codice.CM.Common;
using PlasticGui;
using Unity.PlasticSCM.Editor.Tool;
using Unity.PlasticSCM.Editor.UI;
using UnityEditor;
using UnityEngine;

namespace Unity.PlasticSCM.Editor.AutoSetup
{
    class IncomingChangesFromMainNotification
    {
        public static void Execute()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }

            if (EditorApplication.isPlaying)
            {
                return;
            }
            
            if (mInProgress)
            {
                return;
            }

            double currentUpdateTime = EditorApplication.timeSinceStartup;
            double elapsedSeconds = currentUpdateTime - mLastAutoCommitTime;

            if (elapsedSeconds < mFixedAutoRefresh)
            {
                return;
            }

            mLastAutoCommitTime = currentUpdateTime;
            
            mInProgress = true;

            try
            {
                CheckIncomingChanges();
            }
            finally
            {
                mInProgress = false;
            }
        }

        static void CheckIncomingChanges()
        {
            WorkspaceInfo mWkInfo = FindWorkspace.InfoForApplicationPath(Application.dataPath, Plastic.API);
            
            if (mWkInfo == null)
            {
                return;
            }

            RepositorySpec repositorySpec = Plastic.API.GetRepositorySpec(mWkInfo);
            BranchInfo workingBranchInfo = Plastic.API.GetWorkingBranch(mWkInfo);
            BranchInfo mainBranchInfo = Plastic.API.GetMainBranch(repositorySpec);
           
            if (workingBranchInfo.BranchId == mainBranchInfo.BranchId)
            {
                return;
            }

            string mergesQueryString = "find merge where srcbranch = 'br:{0}' and dstbranch = 'br:{1}'";
            string mergesQuery = string.Format(mergesQueryString, mainBranchInfo.BranchName, workingBranchInfo.BranchName);
            string reverseMergesQuery = string.Format(mergesQueryString, workingBranchInfo.BranchName, mainBranchInfo.BranchName);
            
            IList mergesList = Plastic.API.FindQuery(mWkInfo, mergesQuery).Result[0];
            IList reverseMergesList = Plastic.API.FindQuery(mWkInfo, reverseMergesQuery).Result[0];

            MergeLinkRealizationInfo latestMergeInfo = null;
            if (mergesList.Count > 0)
            {
                latestMergeInfo = (MergeLinkRealizationInfo) mergesList[mergesList.Count - 1];
            }

            if (reverseMergesList.Count > 0)
            {
                MergeLinkRealizationInfo reverseLatestMergeInfo = (MergeLinkRealizationInfo) reverseMergesList[reverseMergesList.Count - 1];
                if (latestMergeInfo == null || reverseLatestMergeInfo.UtcTimeStamp > latestMergeInfo.UtcTimeStamp)
                {
                    latestMergeInfo = reverseLatestMergeInfo;
                }
            }

            if (latestMergeInfo != null)
            {
                ChangesetInfo mainBranchChangeset = latestMergeInfo.SourceObject as ChangesetInfo;

                if (mainBranchChangeset.BranchId != mainBranchInfo.BranchId)
                {
                    mainBranchChangeset = latestMergeInfo.DestinationObject as ChangesetInfo;
                }
                
                if (mainBranchChangeset.ChangesetId == mainBranchInfo.Changeset)
                {
                    mShow = false;
                }
                else
                {
                    mShow = true;
                }

                return;
            }

            string csetsQuery = string.Format("find changesets where changesetid =  {0}", mainBranchInfo.Changeset);
            IList changesets = Plastic.API.FindQuery(mWkInfo, csetsQuery).Result[0];
            ChangesetInfo changesetInfo = (ChangesetInfo)changesets[0];
            
            if (changesetInfo.UtcTimeStamp > workingBranchInfo.UtcTimeStamp)
            {
                mShow = true;
            }
            else
            {
                mShow = false;
            }
        }

        public static void DrawIncomingChangesNotification()
        {
            if (mShow)
            {
                GUIContent labelContent = new GUIContent(
                    "New incoming changes from main branch", "");
                GUIContent buttonContent = new GUIContent(
                    "Merge", "");

                float panelWidth = GetPanelWidth(
                    labelContent, buttonContent,
                    UnityStyles.Notification.Label, EditorStyles.miniButton);

                EditorGUILayout.BeginHorizontal(
                    UnityStyles.Notification.GreenNotification,
                    GUILayout.Width(panelWidth));

                GUILayout.Label(labelContent, UnityStyles.Notification.Label);
                
                DoActionButton(buttonContent, EditorStyles.miniButton);
                
                EditorGUILayout.EndHorizontal();
            }
        }
        
        static void DoActionButton(GUIContent buttonContent, GUIStyle buttonStyle)
        {
            if (!GUILayout.Button(
                buttonContent, buttonStyle,
                GUILayout.ExpandHeight(true),
                GUILayout.MinWidth(40)))
                return;
                
            WorkspaceInfo mWkInfo = FindWorkspace.InfoForApplicationPath(Application.dataPath, Plastic.API);

            LaunchTool.OpenBranchExplorer(mWkInfo);
        }
        static float GetPanelWidth(
            GUIContent labelContent, GUIContent buttonContent,
            GUIStyle labelStyle, GUIStyle buttonStyle)
        {
            return labelStyle.CalcSize(labelContent).x +
                   buttonStyle.CalcSize(buttonContent).x + 12;
        }

        static double mLastAutoCommitTime = 0f;
        const double mFixedAutoRefresh = 5;
        static volatile bool mShow;
        private static bool mInProgress;
    }
    
}