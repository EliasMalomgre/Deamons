package be.kdg.stemtest.model.db.dao

import androidx.room.*
import be.kdg.stemtest.model.entity.Statement
import io.reactivex.Maybe
import io.reactivex.Single

@Dao
interface StatementDao {
    @Insert
    fun insert(stelling: Statement)

    @Delete
    fun delete(stelling: Statement)

    @Query("SELECT * FROM statement")
    fun getAllStatements(): Single<List<Statement>>

    @Query("SELECT * FROM statement WHERE id=:id")
    fun getStatement(id:Int):Maybe<Statement>

    @Query("SELECT IFNULL(COUNT(*),0) FROM statement")
    fun getHighestStatementNumber():Single<Int>
}