package be.kdg.stemtest.model.db.dao

import androidx.room.Dao
import androidx.room.Delete
import androidx.room.Insert
import androidx.room.Query
import be.kdg.stemtest.model.entity.Identification
import io.reactivex.Completable
import io.reactivex.Maybe
import io.reactivex.Single

@Dao
interface IdentificationDao {

    @Insert
    fun insert(identification: Identification) : Completable

    @Delete
    fun delete(identification: Identification)

    @Query("SELECT studentId FROM identification")
    fun getLeerlingCode() : Single<Int>

    @Query("SELECT sessionCode FROM identification")
    fun getSessionCode() : Single<Int>

    @Query("SELECT * FROM identification")
    fun getIdentification() : Maybe<Identification>

}