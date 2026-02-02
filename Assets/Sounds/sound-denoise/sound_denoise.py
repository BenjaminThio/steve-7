import string
from typing import cast
from numpy import ndarray
from scipy.signal import butter, sosfilt
import soundfile as sf
import noisereduce as nr
import os

SOUNDS_FOLDER_PATH = "sounds"
CLEAN_SOUNDS_FOLDER_PATH = "clean_sounds"

def apply_high_pass(data: ndarray, rate: int, cutoff: int = 80) -> ndarray:
    sos: ndarray = cast(ndarray, butter(N=6, Wn=cutoff, btype='highpass', output='sos', fs=rate))

    if data.ndim > 1:
        filtered: ndarray = data.copy()

        for i in range(data.shape[0]):
            filtered[i] = sosfilt(sos, data[i])

        return filtered
    else:
        return cast(ndarray, sosfilt(sos, data))

def main() -> None:
    filename: str

    for filename in os.listdir(SOUNDS_FOLDER_PATH):
        stem: str = filename.split('.')[0]
        data: ndarray
        rate: int
        data, rate = sf.read(f'{SOUNDS_FOLDER_PATH}/{filename}')

        if data.ndim > 1:
            data = data.T

        data = apply_high_pass(data, rate)

        noise_len: int = int(0.5 * rate)
        noise_part: ndarray = data[..., :noise_len]
        reduced_noise: ndarray = nr.reduce_noise(
            y=data,
            sr=rate,
            stationary=True,
            y_noise=noise_part,
            prop_decrease=0.95,
            time_constant_s=2.0,
            n_jobs=1
        )

        if reduced_noise.ndim > 1:
            reduced_noise = reduced_noise.T

        sf.write(f'{CLEAN_SOUNDS_FOLDER_PATH}/{stem}.wav', reduced_noise, rate)

if __name__ == '__main__':
    main()
