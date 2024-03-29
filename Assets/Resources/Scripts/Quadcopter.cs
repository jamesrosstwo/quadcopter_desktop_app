﻿using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public static class Quadcopter
{
    public static QuadcopterData Data = new QuadcopterData();
    private static TcpClient _socket;
    public static bool SocketReady = false;
    public static NetworkStream Stream;
    static StreamWriter _writer;
    static StreamReader _reader;
    public static Vector3 Rotation = Vector3.zero;
    public static String Host = "10.50.127.81";
    public static Int32 Port = 50001;

    public static void SetupSocket()
    {
        try
        {
            _socket = new TcpClient(Host, Port);
            Stream = _socket.GetStream();
            _writer = new StreamWriter(Stream);
            _reader = new StreamReader(Stream);
            SocketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }

    public static void WriteSocket(string theLine)
    {
        if (!SocketReady)
            return;
        String tmpString = theLine;
        _writer.Write(tmpString);
        _writer.Flush();
    }

    public static String ReadSocket()
    {
        if (!SocketReady)
        {
            Debug.LogError("Tried to read from unavailable socket");
            return "";
        }


        string lines = "";
        while (_reader.Peek() != 3)
        {
            lines += (char) (_reader.Read());
        }

        _reader.Read();
        return lines;
    }

    public static IEnumerator PullReadings(float cooldown)
    {
        while (true)
        {
            WriteSocket("readings");
            string json = ReadSocket();
            Data = JsonUtility.FromJson<QuadcopterData>(json);
            yield return new WaitForSeconds(cooldown);
        }
    }

    public static void printStuff()
    {
        WriteSocket("print('a')");
        string json = ReadSocket();
        Debug.Log(json);
    }

    public static void CloseSocket()
    {
        if (!SocketReady)
            return;
        _writer.Close();
        _reader.Close();
        _socket.Close();
        SocketReady = false;
    }
}

public class QuadcopterData
{
    public string status;
    public float altitude;
    public float rotation_x;
    public float rotation_y;
    public float rotation_z;
    public float distance;
    public float batteryPercentage;
}