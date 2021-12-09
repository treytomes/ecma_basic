using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMABasic.Core.Exceptions
{
	class ProgramEndException : RuntimeException
	{
		public ProgramEndException()
			: base("You have reached the END.")
		{
		}
	}
}
