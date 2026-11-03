using System;
using System.ComponentModel;
using System.Diagnostics;

namespace X12_820Explorer.My
{
    internal static partial class MyProject
    {
        internal partial class MyForms
        {

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmX12820main m_frmX12820main;

            public frmX12820main frmX12820main
            {
                [DebuggerHidden]
                get
                {
                    m_frmX12820main = Create__Instance__(m_frmX12820main);
                    return m_frmX12820main;
                }
                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmX12820main))
                        return;
                    if (value is not null)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmX12820main);
                }
            }

        }


    }
}