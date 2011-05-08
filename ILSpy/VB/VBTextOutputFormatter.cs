﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.VB;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.VB
{
	/// <summary>
	/// Description of VBTextOutputFormatter.
	/// </summary>
	public class VBTextOutputFormatter : IOutputFormatter
	{
		readonly ITextOutput output;
		readonly Stack<AstNode> nodeStack = new Stack<AstNode>();
		
		public VBTextOutputFormatter(ITextOutput output)
		{
			if (output == null)
				throw new ArgumentNullException("output");
			this.output = output;
		}
		
		public void StartNode(AstNode node)
		{
//			var ranges = node.Annotation<List<ILRange>>();
//			if (ranges != null && ranges.Count > 0)
//			{
//				// find the ancestor that has method mapping as annotation
//				if (node.Ancestors != null && node.Ancestors.Count() > 0)
//				{
//					var n = node.Ancestors.FirstOrDefault(a => a.Annotation<MemberMapping>() != null);
//					if (n != null) {
//						MemberMapping mapping = n.Annotation<MemberMapping>();
//
//						// add all ranges
//						foreach (var range in ranges) {
//							mapping.MemberCodeMappings.Add(new SourceCodeMapping {
//							                               	ILInstructionOffset = range,
//							                               	SourceCodeLine = output.CurrentLine,
//							                               	MemberMapping = mapping
//							                               });
//						}
//					}
//				}
//			}
			
			nodeStack.Push(node);
		}
		
		public void EndNode(AstNode node)
		{
			if (nodeStack.Pop() != node)
				throw new InvalidOperationException();
		}
		
		public void WriteIdentifier(string identifier)
		{
			MemberReference memberRef = GetCurrentMemberReference();
			
			if (memberRef != null)
				output.WriteReference(identifier, memberRef);
			else
				output.Write(identifier);
		}

		MemberReference GetCurrentMemberReference()
		{
			AstNode node = nodeStack.Peek();
			MemberReference memberRef = node.Annotation<MemberReference>();
			// TODO : implement this!
//			if (memberRef == null && node.Role == AstNode.Roles.TargetExpression && (node.Parent is InvocationExpression || node.Parent is ObjectCreateExpression)) {
//				memberRef = node.Parent.Annotation<MemberReference>();
//			}
			return memberRef;
		}
		
		public void WriteKeyword(string keyword)
		{
			output.Write(keyword);
		}
		
		public void WriteToken(string token)
		{
			// Attach member reference to token only if there's no identifier in the current node.
			MemberReference memberRef = GetCurrentMemberReference();
			if (memberRef != null && nodeStack.Peek().GetChildByRole(AstNode.Roles.Identifier).IsNull)
				output.WriteReference(token, memberRef);
			else
				output.Write(token);
		}
		
		public void Space()
		{
			output.Write(' ');
		}
		
		public void Indent()
		{
			output.Indent();
		}
		
		public void Unindent()
		{
			output.Unindent();
		}
		
		public void NewLine()
		{
			output.WriteLine();
		}
	}
}
