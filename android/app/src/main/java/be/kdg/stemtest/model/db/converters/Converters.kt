package be.kdg.stemtest.model.db.converters

import androidx.room.TypeConverter
import com.google.gson.Gson

class Converters {

    @TypeConverter
    fun fromFloatArray(floatArray: FloatArray): String {
        return Gson().toJson(floatArray)
    }

    @TypeConverter
    fun fromString(json: String): FloatArray {
        return Gson().fromJson(json, FloatArray::class.java)
    }
}