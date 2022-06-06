
from typing import List
import numpy as np
from scipy.integrate import odeint
def solve(arr:list,polyndrom_dict:dict):
    
    a1, a2, a3, a4, a5 = (0.3, 0.6, 0.5, 0.1, 1)
    b1, b2, b3, b4, b5 = (0.75, 0.32, 0.11, 0.9, 0.55)
    c1, c2, c3 = (0.69, 0.28, 0.88)
    def f2(t):
        if t <= 0.3:
            return 0.1
        if t > 0.3:
            return 0.8
    def f3(t):
        if t <= 0.4:
            return 0.1
        if 0.4 < t <= 0.6:
            return 0.5 
        if t > 0.6:
            return 0.9
    def f1(t):
        if t <= 0.2:
            return 0.9
        if 0.2 < t <= 0.7:
            return 0.5
        if t > 0.7:
            return 0.2
    def fak1(t):
        return np.vectorize(f2)(t)
    def fak2(t):
        return np.vectorize(f3)(t)
    def fak3(t):
        return np.vectorize(f1)(t)
    def fun(list):
        return lambda x : list[0]*(x**3)+list[1]*(x**2)+list[2]*x+list[3]
    f = np.empty(36,object)
    for j in range(0, 36):
        if j+1 in polyndrom_dict:
            f[j] = fun(polyndrom_dict[j+1])
        else: f[j] = lambda x : 1
        
    def fu(u, t):
        u1 = ((f[0](u[1]) * f[1](u[4]) * f[2](u[6]))-(fak1(t)))
        u2 = ((f[3](u[7]) * f[4](u[8]))-(f[5](u[9])))
        u3 = ((f[6](u[1]) * f[7](u[4]) * f[8](u[9]))*(fak1(t))-(fak2(t)))
        u4 = ((f[9](u[1]) * f[11](u[4]))-(f[10](u[2]))*(fak3(t)))
        u5 = ((f[12](u[1]) * f[13](u[7]))-(f[14](u[9]))*(fak2(t)))
        u6 = ((f[15](u[0]) * f[16](u[6]))-(f[17](u[9]))*(fak1(t)))
        u7 = ((f[18](u[0]) * f[19](u[5]) * f[21](u[9]))-(f[20](u[7]) * f[22](u[10]))*(fak1(t) + fak2(t)))
        u8 = ((f[23](u[2]) * f[24](u[3]))-(fak2(t) + fak3(t)))
        u9 = ((f[25](u[2]) * f[26](u[3]) * f[28](u[7]) * f[30](u[10]))-(f[27](u[6]) * f[29](u[9]))*(fak3(t)))
        u10 = ((f[31](u[0]) * f[32](u[5]))-(fak1(t) + fak3(t)))
        u11 = ((f[33](u[0]))*(fak1(t))-(f[34](u[6]) * f[35](u[9])))
        return [u1, u2, u3, u4, u5, u6, u7, u8, u9, u10, u11]
    t = np.linspace(0, 1, 11)
    s = odeint(fu, arr, t)
    return s