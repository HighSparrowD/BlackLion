import time

coefficient = 1 # 0% of similarity
coefficientDeviation = 0
speedCoefficient = 10
pointsCount = 0


while not coefficient == 0 and coefficient > 0:
    pointsCount += 1
    coefficientDeviation = (pointsCount * coefficient) / speedCoefficient
    coefficient -= coefficientDeviation / (pointsCount / coefficientDeviation)
    time.sleep(0.1)
    print(f"Similarity percentage: {'{:.10%}'.format(1 - coefficient)}")
    print(f"Coefficient of similarity: {coefficient}")
    print(f"Coefficient deviation: {coefficientDeviation}")
    print(f"Points used: {pointsCount}")
    print("----------------------------------------------------") #37 - 0.11824317778357799