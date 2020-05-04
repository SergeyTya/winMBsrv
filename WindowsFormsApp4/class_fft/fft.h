//---------------------------------------------------------------------------

#ifndef fftH
#define fftH

#include <math.h>
#include "flt.h"

//---------------------------------------------------------------------------

class FFT
{

public:

  long       N;                                    // Размер блока данных
  long       T;                                    // N = 2 ^ T
  long       No;       
  double     CFreq;   
  int        inversed;                             // Признак инверсной подачи данных 

  double     X0, tm0;
  double     **Xre, **Xim;
  double     *tm, dt;                              // Время, приращение времени
  long       **ind;                                // Индексы для перестановок
  double     **Wre, **Wim;                         // Поворачивающие множители
  double     *A, *freq;                            // Амплитуды, частоты
  double     dT;
  bool       ReadyToBegin, Fault, Otsechka;

  double     *EVENre, *ODDre, *EVENim, *ODDim;     // Буфера для отображений Фурье четных и нечетных наборов


  Filter     Flt;
  double     fH, fT;                               // Параметры фильтра



  FFT(int t);       // Конструктор
  ~FFT();           // Деструктор


  void __fastcall    Prepare(void);
  bool __fastcall    PutVal(bool c);
  float __fastcall   Spectrum(void);               // Возвращает погрешность зеркального эффекта

};

//---------------------------------------------------------------------------


#endif
