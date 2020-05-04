//---------------------------------------------------------------------------

#ifndef fftH
#define fftH

#include <math.h>
#include "flt.h"

//---------------------------------------------------------------------------

class FFT
{

public:

  long       N;                                    // ������ ����� ������
  long       T;                                    // N = 2 ^ T
  long       No;       
  double     CFreq;   
  int        inversed;                             // ������� ��������� ������ ������ 

  double     X0, tm0;
  double     **Xre, **Xim;
  double     *tm, dt;                              // �����, ���������� �������
  long       **ind;                                // ������� ��� ������������
  double     **Wre, **Wim;                         // �������������� ���������
  double     *A, *freq;                            // ���������, �������
  double     dT;
  bool       ReadyToBegin, Fault, Otsechka;

  double     *EVENre, *ODDre, *EVENim, *ODDim;     // ������ ��� ����������� ����� ������ � �������� �������


  Filter     Flt;
  double     fH, fT;                               // ��������� �������



  FFT(int t);       // �����������
  ~FFT();           // ����������


  void __fastcall    Prepare(void);
  bool __fastcall    PutVal(bool c);
  float __fastcall   Spectrum(void);               // ���������� ����������� ����������� �������

};

//---------------------------------------------------------------------------


#endif
