using UnityEngine;
using System.Collections.Generic;

public class ObstacleFadeManager : MonoBehaviour
{
    public Transform viewer; // 기준 위치 (예: 플레이어 또는 카메라)
    public float fadeStartZ = 0f; // 이 Z좌표를 지나면 페이드 시작
    public float fadeDistance = 5f; // 페이드가 완료되는 거리 범위
    public float targetAlpha = 0.2f; // 최종 알파값

    private class FadeData
    {
        public Renderer renderer;
        public float currentAlpha = 1f;
    }

    private List<FadeData> fadeTargets = new();
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        foreach (var fade in fadeTargets)
        {
            if (fade.renderer == null) continue;

            float obstacleZ = fade.renderer.transform.position.z;
            float viewerZ = viewer.position.z;

            // 기준 Z보다 뒤에 있을 경우만 페이드 적용
            if (obstacleZ >= fadeStartZ)
            {
                ApplyAlpha(fade.renderer, 1f); // 완전 불투명 유지
                continue;
            }

            float zDiff = viewerZ - obstacleZ;
            float t = Mathf.InverseLerp(0f, fadeDistance, zDiff);
            float target = Mathf.Lerp(1f, targetAlpha, t);

            fade.currentAlpha = Mathf.MoveTowards(fade.currentAlpha, target, Time.deltaTime);
            ApplyAlpha(fade.renderer, fade.currentAlpha);
        }
    }


    public void RegisterObstacle(Renderer rend)
    {
        if (rend == null) return;
        fadeTargets.Add(new FadeData { renderer = rend });
    }

    private void ApplyAlpha(Renderer rend, float alpha)
    {
        rend.GetPropertyBlock(mpb);
        Color color = Color.white;

        if (rend.sharedMaterial.HasProperty("_BaseColor"))
            color = rend.sharedMaterial.GetColor("_BaseColor");
        else if (rend.sharedMaterial.HasProperty("_Color"))
            color = rend.sharedMaterial.GetColor("_Color");

        color.a = alpha;

        if (rend.sharedMaterial.HasProperty("_BaseColor"))
            mpb.SetColor("_BaseColor", color);
        else if (rend.sharedMaterial.HasProperty("_Color"))
            mpb.SetColor("_Color", color);

        rend.SetPropertyBlock(mpb);
    }
}
