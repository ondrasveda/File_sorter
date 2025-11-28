def calc_voltage(I: float, R: float) -> float:
    return I * R

def calc_resistance(V: float, I: float) -> float:
    return V / I

def calc_current(V: float, R: float) -> float:
    return V / R

def calc_coulomb(q1: float, q2: float, r: float, er: float) -> float:
    # konstanty
    E0 = 0.00000000000885
    # vypocet
    epsilon = E0 * er
    return (1 / (4 * 3.14 * epsilon)) * abs(q1 * q2) / (r ** 2)  # Sila v Newtone