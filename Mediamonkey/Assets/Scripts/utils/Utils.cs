/**
 * Copyright (c) 2011 Bart Wttewaall
 * Website: http://www.mediamonkey.nl
 */

using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

/**
 * Usage
 * Add this in the constructor or the Awake or Start handler to exclude all traces from that class:
 	* Utils.ignoreClass(this);
 * 
 * get caller name with Regex:
 	* using System.Text.RegularExpressions;
 	* private static Regex callerPattern = new Regex(@"\s*at (.+?)(?=\.).*", RegexOptions.Singleline);
 	* string caller = callerPattern.Replace(new StackTrace(1).ToString(), "$1");
 * 
 * get caller name with GetMethod():
 	* string caller = new StackFrame(1).GetMethod().DeclaringType.FullName;
 * 
 */

public sealed class Utils {
	
	/// <summary>
	/// toggle ALL traces
	/// </summary>
	public static bool verbose = true;
	
	/// <summary>
	/// toggle stacktrace output for trace()
	/// </summary>
	public static bool showStackTrace = false;
	
	/// <summary>
	/// ignore caller if its class has been listed
	/// </summary>
	private static List<string> ignoreList = new List<string>();
	
	// ---- public static methods ----
	
	/// <summary>
	/// Add an instance's classname to the ignorelist
	/// </summary>
	/// <param name="instance">
	/// A <see cref="System.Object"/>
	/// </param>
	public static void ignoreClass(object instance) {
		string caller = new StackFrame(1).GetMethod().DeclaringType.FullName;
		if (!ignoreList.Contains(caller)) ignoreList.Add(caller);
	}
	
	/// <summary>
	/// Output multiple parameters to Debug.Log()
	/// </summary>
	/// <param name="args">
	/// A <see cref="System.Object[]"/>
	/// </param>
	public static void trace(params object[] args) {
		if (!verbose) return;
		
		// ignore caller?
		string caller = new StackFrame(1).GetMethod().DeclaringType.FullName;
		if (ignoreList.Contains(caller)) return;
		
		string output = getOutput(args);
		
		if (showStackTrace) {
			// show relevant stacktrace info
			string stack = StackTraceUtility.ExtractStackTrace();
			stack = stack.Substring(stack.IndexOf("\n")); // remove first line
			UnityEngine.Debug.Log(output+stack);
			
		} else {
			// add newline for a clean output
			UnityEngine.Debug.Log(output+"\n");
		}
	}
	
	/// <summary>
	/// Output multiple parameters to Debug.Log() with additional StackTrace information
	/// </summary>
	/// <param name="args">
	/// A <see cref="System.Object[]"/>
	/// </param>
	public static void traceStack(params object[] args) {
		if (!verbose) return;
		
		// ignore caller?
		string caller = new StackFrame(1).GetMethod().DeclaringType.FullName;
		if (ignoreList.Contains(caller)) return;
		
		string output = getOutput(args);
		
		// show relevant stacktrace info
		string stack = StackTraceUtility.ExtractStackTrace();
		stack = stack.Substring(stack.IndexOf("\n")); // remove first line
		UnityEngine.Debug.Log(output+stack);
	}
	
	public static void warn(params object[] args) {
		UnityEngine.Debug.LogWarning(getOutput(args)+"\n");
	}
	
	public static void error(params object[] args) {
		UnityEngine.Debug.LogError(getOutput(args)+"\n");
	}
	
	// ---- private static methods ----
	
	private static string getOutput(object[] args) {
		string output = "";
		
		for (int i=0; i<args.Length; i++) {
			string argument = (args[i] == null) ? "Null" : args[i].ToString();
			output += argument + (i < args.Length-1 ? " " : "");
		}
		
		return output;
	}
	
}