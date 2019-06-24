#ifndef GSLINE_HLSL
#define GSLINE_HLSL
#define MATRIXSPLINE
#define MVC 56
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

void makeLineRibbonAdj(
    out float2 ribbon[4], 
    in float2 pos0, in float2 pos1, in float2 pos2, in float2 pos3, 
    in float width)
{
    // Compute tangent and binormal of the current line in window space
    float2 currT = normalize(pos2 - pos1);
    float2 currB = float2(currT.y, -currT.x);

    // Binormal is scaled by line width
    float2 currBs = currB * width; 
    
    // Compute the corners of the current ribbon in window space
    float2 currA1 = pos1 - currBs;
    float2 currA2 = pos1 + currBs;
    float2 currB1 = pos2 - currBs;
    float2 currB2 = pos2 + currBs;

    //if (all(pos0 != pos1))
    //{
    //    // Compute tangent and binormal of the previous line in window space
    //    float2 prevT = normalize(pos1 - pos0);
    //    float2 prevB = float2(prevT.y, -prevT.x);
    //
    //    // Binormal is scaled by line width
    //    float2 prevBs = prevB * width;
    //
    //    // Compute the end corners of the previous ribbon in window space
    //    float2 prevB1 = pos1 - prevBs;
    //    float2 prevB2 = pos1 + prevBs;
    //
    //    // Connect to the previous ribbon through averaging
    //    currA1 = (currA1 + prevB1) / 2.0;
    //    currA2 = (currA2 + prevB2) / 2.0;
    //}
    //
    //if (all(pos2 != pos3))
    //{
    //    // Compute tangent and binormal of the next line in window space
    //    float2 nextT = normalize(pos3 - pos2);
    //    float2 nextB = float2(nextT.y, -nextT.x);
    //
    //    // Binormal is scaled by line width
    //    float2 nextBs = nextB * width;
    //
    //    // Compute the end corners of the next ribbon in window space
    //    float2 nextA1 = pos2 - nextBs;
    //    float2 nextA2 = pos2 + nextBs;
    //
    //    // Connect to the next ribbon through averaging
    //    currB1 = (currB1 + nextA1) / 2.0;
    //    currB2 = (currB2 + nextA2) / 2.0;
    //}

    ribbon[1] = currA1;
    ribbon[0] = currA2;
    ribbon[3] = currB1;
    ribbon[2] = currB2;
}

void makeLineAdjFixed(
    out float4 ribbon[4], 
    in float4 pos0, in float4 pos1, in float4 pos2, in float4 pos3, 
    in float width)
{
    // Bring pos0..pos3 into window space
    float2 p0w = projToWindow(pos0);
    float2 p1w = projToWindow(pos1);
    float2 p2w = projToWindow(pos2);
    float2 p3w = projToWindow(pos3);

    float2 ribbon2D[4];
    makeLineRibbonAdj(ribbon2D, p0w, p1w, p2w, p3w, width);

    // Bring back corners into projection space
    ribbon[0] = windowToProj(ribbon2D[0], pos0.z, pos0.w);
    ribbon[1] = windowToProj(ribbon2D[1], pos1.z, pos1.w);
    ribbon[2] = windowToProj(ribbon2D[2], pos2.z, pos2.w);
    ribbon[3] = windowToProj(ribbon2D[3], pos3.z, pos3.w);
}

void makeLineAdjNonFixed(
    out float4 ribbon[4], 
    in float4 pos0, in float4 pos1, in float4 pos2, in float4 pos3, 
    in float width)
{
    // Bring pos0..pos3 into view space
    float4 pos0v = mul(pos0, mView);
    float4 pos1v = mul(pos1, mView);
    float4 pos2v = mul(pos2, mView);
    float4 pos3v = mul(pos3, mView);

    float2 ribbon2D[4];
    makeLineRibbonAdj(ribbon2D, pos0v, pos1v, pos2v, pos3v, width);

    // bring back corners in projection frame
    ribbon[0] = mul(float4(ribbon2D[0], pos1v.z, pos1v.w), mProjection);
    ribbon[1] = mul(float4(ribbon2D[1], pos1v.z, pos1v.w), mProjection);
    ribbon[2] = mul(float4(ribbon2D[2], pos2v.z, pos2v.w), mProjection);
    ribbon[3] = mul(float4(ribbon2D[3], pos2v.z, pos2v.w), mProjection);
}

[maxvertexcount(MVC)]
void main(lineadj GSInputPS input[4], inout TriangleStream<PSInputPS> outStream)
{
    PSInputPS output = (PSInputPS) 0;

    float4 lineCorners[4] = { (float4) 0, (float4) 0, (float4) 0, (float4) 0 };

    int num = MVC / 4;
    float dt = 1.0 / float(num);

    float4 p0, p1, p2, p3;
    if (fixedSize)
    {
        p0 = input[0].p;
        p1 = input[1].p;
        p2 = input[2].p;
        p3 = input[3].p;
    }
    else
    {
        p0 = input[0].wp;
        p1 = input[1].wp;
        p2 = input[2].wp;
        p3 = input[3].wp;
    }

    const float4x4 p = { p0, p1, p2, p3 };
    //const float4x4 c = { input[0].c, input[1].c, input[2].c, input[3].c };
    const float4x4 mp = mul(mSpline, p);
    //const float4x4 mc = mul(mSpline, c);

    float t = dt;
    float4 pos0 = p0;
    float4 pos1 = mp[3];
    float4 col1 = input[1].c; // mc[3];
    float4 pos2 = (((mp[0] * t + mp[1]) * t + mp[2]) * t + mp[3]);
 
    for (int i = 1; i <= num; i++)
    {
        float4 col2 = t * input[2].c + (1-t) * input[1].c;
        //float4 col2 = (((mc[0] * t + mc[1]) * t + mc[2]) * t + mc[3]);
        t += dt;
        float4 pos3 = (((mp[0] * t + mp[1]) * t + mp[2]) * t + mp[3]);

        if (fixedSize)
            makeLineAdjFixed(lineCorners, pos0, pos1, pos2, pos3, pfParams.x);
        else
            makeLineAdjNonFixed(lineCorners, pos0, pos1, pos2, pos3, pfParams.x);

        output.vEye = input[1].vEye;
        output.c = col1;
        output.p = lineCorners[0];
        output.t = float3(1, 1, 1);   
        outStream.Append(output);
        
        output.p = lineCorners[1];
        output.t = float3(1, -1, 1);
        outStream.Append(output);
           
        output.vEye = input[2].vEye;
        output.c = col2;
        output.p = lineCorners[2];
        output.t = float3(-1, 1, 1);
        outStream.Append(output);
        
        output.p = lineCorners[3];
        output.t = float3(-1, -1, 1);
        outStream.Append(output);

        pos0 = pos1;
        pos1 = pos2;
        pos2 = pos3;
        col1 = col2;
    }

    outStream.RestartStrip();
}

#endif