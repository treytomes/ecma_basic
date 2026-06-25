using ECMABasic.Domain;
using ECMABasic.Domain.Expressions;
﻿using ECMABasic.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace ECMABasic.Application;

public class NumericExpressionParser : ExpressionParser
{
	public NumericExpressionParser(ComplexTokenReader reader, int? lineNumber, bool throwOnError)
		: base(reader, lineNumber, throwOnError)
	{
	}

	public override IExpression? Parse()
	{
		var expr = ParseBoolean();
		return expr;
	}

	private IExpression? ParseBoolean()
	{
		var left = ParseSums();
		if (left == null)
		{
			return null;
		}

		var space = _reader.Next(TokenType.Space, false);

		var symbol = ParseBooleanOperator();
		if (symbol == null)
		{
			if (space != null)
			{
				_reader.Rewind();
			}
			return left;
		}

		_reader.Next(TokenType.Space, false);

		IExpression? right;
		try
		{
			right = ParseSums();
		}
		catch (SyntaxException)
		{
			if (new StringExpressionParser(_reader, _lineNumber, false).Parse() != null)
			{
				throw ExceptionFactory.MixedStringsAndNumbers(_lineNumber);
			}
			else
			{
				throw;
			}
		}

		if (right == null)
		{
			throw ExceptionFactory.ExpectedNumericExpression(_lineNumber);
		}

		try
		{
			if (symbol.Text == "=")
			{
				return new EqualsExpression(left, right);
			}
			else if (symbol.Text == "<>")
			{
				return new NotEqualsExpression(left, right);
			}
			else if (symbol.Text == "<")
			{
				return new LessThanExpression(left, right);
			}
			else if (symbol.Text == "<=")
			{
				return new LessThanOrEqualExpression(left, right);
			}
			else if (symbol.Text == ">")
			{
				return new GreaterThanExpression(left, right);
			}
			else if (symbol.Text == ">=")
			{
				return new GreaterThanOrEqualExpression(left, right);
			}
			else
			{
				throw ExceptionFactory.IllegalOperator(_lineNumber);
			}
		}
		catch (SyntaxException ex)
		{
			throw new SyntaxException(ex, _lineNumber);
		}
	}

	private IExpression? ParseSums()
	{
		var expr = ParseProducts();
		if (expr == null)
		{
			return expr;
		}

		while (true)
		{
			var space = _reader.Next(TokenType.Space, false);

			var preSymbolIndex = _reader.TokenIndex;
			var symbol = _reader.Next(TokenType.Symbol, false, @"\+|\-");
			if (symbol == null)
			{
				if (space != null)
				{
					_reader.Rewind();
				}
				return expr;
			}

			_reader.Next(TokenType.Space, false);

			var right = ParseProducts();
			if (right == null)
			{
				_reader.Seek(preSymbolIndex);
				return expr;
			}

			expr = symbol.Text switch
			{
				"+" => new AdditionExpression(expr, right),
				"-" => new SubtractionExpression(expr, right),
				_ => throw ExceptionFactory.IllegalOperator(_lineNumber),
			};
		}
	}

	private IExpression? ParseProducts()
	{
		var expr = ParseUnary();
		if (expr == null)
		{
			return expr;
		}

		while (true)
		{
			var space = _reader.Next(TokenType.Space, false);

			var preSymbolIndex = _reader.TokenIndex;
			var symbol = _reader.Next(TokenType.Symbol, false, @"\*|\/");
			if (symbol == null)
			{
				if (space != null)
				{
					_reader.Rewind();
				}
				return expr;
			}

			_reader.Next(TokenType.Space, false);

			var right = ParseUnary();
			if (right == null)
			{
				_reader.Seek(preSymbolIndex);
				return expr;
			}

			expr = symbol.Text switch
			{
				"*" => new MultiplicationExpression(expr, right),
				"/" => new DivisionExpression(expr, right),
				_ => throw ExceptionFactory.IllegalOperator(_lineNumber),
			};
		}
	}

	private IExpression? ParseUnary()
	{
		var unaryMinusToken = _reader.Next(TokenType.Symbol, false, @"\-");
		if (unaryMinusToken == null)
		{
			// Read the unary plus if it's there, but don't do anything with it.
			_reader.Next(TokenType.Symbol, false, @"\+");
		}

		_reader.Next(TokenType.Space, false);

		var expr = ParseInvolution();
		if (unaryMinusToken != null)
		{
			if (expr == null)
			{
				throw ExceptionFactory.ExpectedNumericExpression(_lineNumber);
			}
			expr = new NegationExpression(expr);
		}

		return expr;
	}

	private IExpression? ParseInvolution()
	{
		var expr = ParseAtomic(false);
		if (expr == null)
		{
			return expr;
		}

		while (true)
		{
			var space = _reader.Next(TokenType.Space, false);

			var preSymbolIndex = _reader.TokenIndex;
			var symbol = _reader.Next(TokenType.Symbol, false, @"\^|\*");
			if (symbol == null)
			{
				if (space != null)
				{
					_reader.Rewind();
				}
				return expr;
			}
			else if (symbol.Text == "*")
			{
				var next = _reader.Next(TokenType.Symbol, false, @"\*");
				if (next == null)
				{
					_reader.Rewind();
					return expr;
				}
				// At this point we've read the "**" operator.
			}

			_reader.Next(TokenType.Space, false);

			var right = ParseAtomic(true);
			if (right == null)
			{
				_reader.Seek(preSymbolIndex);
				return expr;
			}

			expr = new InvolutionExpression(expr, right);
		}
	}

	private IExpression? ParseAtomic(bool throwOnError)
	{
		var openParenthesis = _reader.Next(TokenType.OpenParenthesis, false);
		if (openParenthesis != null)
		{
			var expr = Parse();
			try
			{
				_reader.Next(TokenType.CloseParenthesis);
			}
			catch (UnexpectedTokenException)
			{
				throw ExceptionFactory.IllegalFormula(_lineNumber);
			}
			return expr;
		}
		else
		{
			// Try function before variable to catch FNA, FNB, etc.
			var expr = ParseLiteral() ?? ParseFunction(false) ?? ParseVariable();
			if (expr == null && throwOnError)
			{
				// If we're in error mode and nothing parsed, try function with error reporting
				expr = ParseFunction(throwOnError);
			}
			if (expr == null)
			{
				if (throwOnError)
				{
					throw ExceptionFactory.ExpectedNumericExpression(_lineNumber);
				}
				else
				{
					return expr;
				}
			}

			return expr;
		}
	}

	public IExpression? ParseVariable()
	{
		var nameToken = _reader.Next(TokenType.Word, false, @"[A-Z]\d?");
		if (nameToken == null)
		{
			return null;
		}

		return new VariableExpression(nameToken.Text);
	}

	public IExpression? ParseLiteral()
	{
		var nextValue = _reader.NextNumber(false);
		if (!nextValue.HasValue)
		{
			return null;
		}
		return new NumberExpression(nextValue.Value);
	}

	public IExpression? ParseFunction(bool throwOnError)
	{
		var startIndex = _reader.TokenIndex;
		var nameToken = _reader.Next(TokenType.Word, false);
		if (nameToken == null)
		{
			return null;
		}

		// Check for user-defined function (FN + letter)
		// Could be "FN A" (two tokens) or "FNA" (one token)
		if (nameToken.Text.Length == 3 &&
		    nameToken.Text.Substring(0, 2).ToUpper() == "FN" &&
		    char.IsLetter(nameToken.Text[2]))
		{
			// Single token: "FNA"
			var functionName = nameToken.Text.ToUpper();

			// Check if there's an argument: FNA(5)
			var openParen = _reader.Next(TokenType.OpenParenthesis, false);
			IExpression? argument = null;

			if (openParen != null)
			{
				argument = Parse();
				_reader.Next(TokenType.CloseParenthesis);
			}

			// Return a user function call expression
			return new UserFunctionCallExpression(functionName, argument, _lineNumber);
		}
		else if (nameToken.Text.ToUpper() == "FN")
		{
			// Two tokens: "FN" "A"
			var letterToken = _reader.Next(TokenType.Word, false);
			if (letterToken != null && letterToken.Text.Length == 1 && char.IsLetter(letterToken.Text[0]))
			{
				var functionName = "FN" + letterToken.Text.ToUpper();

				// Check if there's an argument: FNA(5)
				var openParen = _reader.Next(TokenType.OpenParenthesis, false);
				IExpression? argument = null;

				if (openParen != null)
				{
					argument = Parse();
					_reader.Next(TokenType.CloseParenthesis);
				}

				// Return a user function call expression
				return new UserFunctionCallExpression(functionName, argument, _lineNumber);
			}
			else
			{
				// Not a function call, backtrack
				_reader.Seek(startIndex);
				return null;
			}
		}

		// Not a user function (didn't start with FN)
		// Check if there's an open parenthesis - if not, it's probably a variable, not a function
		var openParen2 = _reader.Next(TokenType.OpenParenthesis, false);
		if (openParen2 == null)
		{
			// No parenthesis - might be parameterless intrinsic (like RND)
			// Or might be a variable - let intrinsic registry decide
			// But first check if it's a known intrinsic
			var env = Interpreter.CurrentParsingEnvironment;
			if (env == null)
			{
				throw new InvalidOperationException("No parsing environment available");
			}

			// Check if this is a known parameterless intrinsic
			var hasIntrinsic = false;
			foreach (var fndef in env.Intrinsics.Get(nameToken.Text))
			{
				if (fndef.CanInstantiate(new List<IExpression>()))
				{
					hasIntrinsic = true;
					break;
				}
			}

			if (!hasIntrinsic)
			{
				// Not an intrinsic function - backtrack and let it be parsed as variable
				_reader.Seek(startIndex);
				return null;
			}

			// It's a parameterless intrinsic function - continue
		}

		// Parse intrinsic function arguments

		var args = new List<IExpression>();

		// Only parse arguments if parentheses are present
		if (openParen2 != null)
		{
			while (true)
			{
				var argExpr = new StringExpressionParser(_reader, _lineNumber, false).Parse();
				if (argExpr == null)
				{
					argExpr = Parse();
					if (argExpr == null)
					{
						break;
					}
				}

				args.Add(argExpr);

				var comma = _reader.Next(TokenType.Comma, false);
				if (comma == null)
				{
					break;
				}
			}

			_reader.Next(TokenType.CloseParenthesis);
		}

		// Access intrinsic registry from current parsing environment
		var environment = Interpreter.CurrentParsingEnvironment;
		if (environment == null)
		{
			throw new InvalidOperationException("No parsing environment available");
		}

		foreach (var fndef in environment.Intrinsics.Get(nameToken.Text))
		{
			if (fndef.CanInstantiate(args))
			{
				return fndef.Instantiate(args, _lineNumber);
			}
		}

		if (throwOnError)
		{
			throw ExceptionFactory.UndefinedFunction(_lineNumber);
		}
		else
		{
			return null;
		}
	}
}
